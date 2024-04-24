using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Python.Runtime;
using SpacyDotNet;

namespace SpaCyDotNet.api
{
    public class Lang : IXmlSerializable
    {
        private List<string> _pipeNames;

        internal Lang(dynamic lang)
        {
            PyLang = lang;
            _pipeNames = null;
            var pipelineMeta = new PipelineMeta(this);
        }

        public Doc GetDocument(string text)
        {
            using (Py.GIL())
            {
                var pyString = new PyString(text);
                var doc = PyLang.__call__(pyString);
                return new Doc(doc, text);
            }
        }

        private dynamic PyLang { get; }

        public IEnumerable<string> PipeNames => Interop.GetListFromList<string>(PyLang?.pipe_names, ref _pipeNames);

        public Vocab Vocab => new Vocab(PyLang.vocab);

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            var dummyBytes = new byte[1];

            Debug.Assert(reader.Name == $"{Serialization.Prefix}:PyObj");
            var bytesB64 = reader.ReadElementContentAsString();
            var bytes = Convert.FromBase64String(bytesB64);
            using (Py.GIL())
            {
                var pyBytes = ToPython.GetBytes(bytes);
                PyLang.from_bytes(pyBytes);
            }

            Debug.Assert(reader.Name == $"{Serialization.Prefix}:PipeNames");
            var pipeNames = reader.ReadElementContentAsString();
            _pipeNames = pipeNames.Split(',').ToList();

            // TODO: Yet to debug. It's not being used so far
            new PipelineMeta(this);
        }

        public void WriteXml(XmlWriter writer)
        {
            using (Py.GIL())
            {
                var pyObj = Interop.GetBytes(PyLang.to_bytes());
                var pyObjB64 = Convert.ToBase64String(pyObj);
                writer.WriteElementString("PyObj", pyObjB64, Serialization.Namespace);
            }

            // Using the property is important form the members to be loaded
            writer.WriteElementString("PipeNames", string.Join(',', PipeNames), Serialization.Namespace);
        }

        private class PipelineMeta(Lang lang) : Dictionary<string, object>
        {
            public new object this[string key]
            {
                get
                {
                    if (ContainsKey(key))
                        return base[key];

                    if (lang.PyLang == null)
                        return null;

                    object ret;
                    using (Py.GIL())
                    {
                        var pyKeyStr = new PyString(key);
                        var pyObj = (PyObject)lang.PyLang.meta.__getitem__(pyKeyStr);

                        if (!PyString.IsStringType(pyObj))
                            throw new NotImplementedException();

                        var pyValStr = new PyString(pyObj);
                        ret = pyValStr.ToString();
                        Add(key, ret);
                    }

                    return ret;
                }
            }
        }
    }
}
