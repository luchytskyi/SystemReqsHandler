﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Python.Runtime;
using SpacyDotNet;

namespace SpaCyDotNet.api
{
    public class Token : IXmlSerializable
    {
        private string _text;
        private string _lemma;

        private string _pos;
        private string _tag;
        private string _dep;
        private string _shape;

        private bool? _isAlpha;
        private bool? _isStop;
        private bool? _isPunct;
        private bool? _isDigit;
        private bool? _likeNum;
        private bool? _likeEMail;

        private bool? _hasVector;
        private double? _vectorNorm;
        private bool? _isOov;

        private int? _i;

        private Token _head;
        private int _headPos;

        private List<Token> _children;

        public Token()
        {
            // Needed to use generics
        }

        private Token(dynamic token)
        {
            _isAlpha = null;
            _isStop = null;
            _isPunct = null;
            _isDigit = null;
            _likeNum = null;
            _likeEMail = null;

            PyObj = token;
        }

        private dynamic PyObj { get; }

        public string Text => Interop.GetString(PyObj?.text, ref _text);

        public string Lemma => Interop.GetString(PyObj?.lemma_, ref _lemma);

        public string PoS => Interop.GetString(PyObj?.pos_, ref _pos);

        public string Tag => Interop.GetString(PyObj?.tag_, ref _tag);

        public string Dep => Interop.GetString(PyObj?.dep_, ref _dep);

        public string Shape => Interop.GetString(PyObj?.shape_, ref _shape);

        public bool IsAlpha => Interop.GetBool(PyObj?.is_alpha, ref _isAlpha);

        public bool IsStop => Interop.GetBool(PyObj?.is_stop, ref _isStop);

        public bool IsPunct => Interop.GetBool(PyObj?.is_punct, ref _isPunct);

        public bool IsDigit => Interop.GetBool(PyObj?.is_digit, ref _isDigit);

        public bool LikeNum => Interop.GetBool(PyObj?.like_num, ref _likeNum);

        public bool LikeEMail => Interop.GetBool(PyObj?.like_email, ref _likeEMail);

        private bool HasVector => Interop.GetBool(PyObj?.has_vector, ref _hasVector);

        private double VectorNorm => Interop.GetDouble(PyObj?.vector_norm, ref _vectorNorm);

        private bool IsOov => Interop.GetBool(PyObj?.is_oov, ref _isOov);

        private int I => Interop.GetInt(PyObj?.i, ref _i);

        public Token Head
        {
            get
            {
                if (_head != null)
                    return _head;

                using (Py.GIL())
                {
                    var pyHeadIsSelf = new PyInt(PyObj.head.__eq__(PyObj));
                    var headIsSelf = pyHeadIsSelf.ToInt32() != 0;
                    _head = headIsSelf ? this : new Token(PyObj.head);

                    return _head;
                }
            }

            set
            {
                _head = value;
            }
        }

        public List<Token> Children => Interop.GetListFromGenerator(PyObj?.children, ref _children);

        public double Similarity(Token token)
        {
            using (Py.GIL())
            {                
                var similarityPy = PyObj.similarity(token.PyObj);
                var similarityPyFloat = PyFloat.AsFloat(similarityPy);
                return similarityPyFloat.As<double>();
            }
        }

        public override string ToString()
        {
            return Text;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Debug.Assert(reader.Name == $"{Serialization.Prefix}:Text");
            _text = reader.ReadElementContentAsString();
            Debug.Assert(reader.Name == $"{Serialization.Prefix}:Lemma");
            _lemma = reader.ReadElementContentAsString();

            Debug.Assert(reader.Name == $"{Serialization.Prefix}:Pos");
            _pos = reader.ReadElementContentAsString();
            Debug.Assert(reader.Name == $"{Serialization.Prefix}:Tag");
            _tag = reader.ReadElementContentAsString();
            Debug.Assert(reader.Name == $"{Serialization.Prefix}:Dep");
            _dep = reader.ReadElementContentAsString();
            Debug.Assert(reader.Name == $"{Serialization.Prefix}:Shape");
            _shape = reader.ReadElementContentAsString();

            Debug.Assert(reader.Name == $"{Serialization.Prefix}:IsAlpha");
            _isAlpha = reader.ReadElementContentAsBoolean();
            Debug.Assert(reader.Name == $"{Serialization.Prefix}:IsStop");
            _isStop = reader.ReadElementContentAsBoolean();
            Debug.Assert(reader.Name == $"{Serialization.Prefix}:IsPunct");
            _isPunct = reader.ReadElementContentAsBoolean();
            Debug.Assert(reader.Name == $"{Serialization.Prefix}:IsDigit");
            _isDigit = reader.ReadElementContentAsBoolean();
            Debug.Assert(reader.Name == $"{Serialization.Prefix}:LikeNum");
            _likeNum = reader.ReadElementContentAsBoolean();
            Debug.Assert(reader.Name == $"{Serialization.Prefix}:LikeEMail");
            _likeEMail = reader.ReadElementContentAsBoolean();

            Debug.Assert(reader.Name == $"{Serialization.Prefix}:HasVector");
            _hasVector = reader.ReadElementContentAsBoolean();
            Debug.Assert(reader.Name == $"{Serialization.Prefix}:VectorNorm");
            _vectorNorm = reader.ReadElementContentAsDouble();
            Debug.Assert(reader.Name == $"{Serialization.Prefix}:IsOov");
            _isOov = reader.ReadElementContentAsBoolean();

            Debug.Assert(reader.Name == $"{Serialization.Prefix}:I");
            _i = reader.ReadElementContentAsInt();

            Debug.Assert(reader.Name == $"{Serialization.Prefix}:Head");
            var headPosStr = reader.GetAttribute("Pos");
            if (string.IsNullOrEmpty(headPosStr))
                _headPos = -1;
            else
                _headPos = int.Parse(headPosStr);
                
            reader.Skip();
        }

        public void WriteXml(XmlWriter writer)
        {
            // Using the property is important form the members to be loaded
            writer.WriteElementString("Text", Serialization.Namespace, Text);
            writer.WriteElementString("Lemma", Serialization.Namespace, Lemma);

            writer.WriteElementString("Pos", Serialization.Namespace, PoS);
            writer.WriteElementString("Tag", Serialization.Namespace, Tag);
            writer.WriteElementString("Dep", Serialization.Namespace, Dep);
            writer.WriteElementString("Shape", Serialization.Namespace, Shape);

            writer.WriteStartElement("IsAlpha", Serialization.Namespace);
            writer.WriteValue(IsAlpha);
            writer.WriteEndElement();
            writer.WriteStartElement("IsStop", Serialization.Namespace);
            writer.WriteValue(IsStop);
            writer.WriteEndElement();
            writer.WriteStartElement("IsPunct", Serialization.Namespace);
            writer.WriteValue(IsPunct);
            writer.WriteEndElement();
            writer.WriteStartElement("IsDigit", Serialization.Namespace);
            writer.WriteValue(IsDigit);
            writer.WriteEndElement();
            writer.WriteStartElement("LikeNum", Serialization.Namespace);
            writer.WriteValue(LikeNum);
            writer.WriteEndElement();
            writer.WriteStartElement("LikeEMail", Serialization.Namespace);
            writer.WriteValue(LikeEMail);
            writer.WriteEndElement();

            writer.WriteStartElement("HasVector", Serialization.Namespace);
            writer.WriteValue(HasVector);
            writer.WriteEndElement();
            writer.WriteStartElement("VectorNorm", Serialization.Namespace);
            writer.WriteValue(VectorNorm);
            writer.WriteEndElement();
            writer.WriteStartElement("IsOov", Serialization.Namespace);
            writer.WriteValue(IsOov);
            writer.WriteEndElement();

            writer.WriteStartElement("I", Serialization.Namespace);
            writer.WriteValue(I);
            writer.WriteEndElement();

            writer.WriteStartElement("Head", Serialization.Namespace);
            var head = Head;
            writer.WriteAttributeString("Pos", head == this ? string.Empty : head.I.ToString());
            writer.WriteEndElement();

            // This one was already commented
            //info.AddValue("Children", Children);
        }

        internal void RestoreHead(List<Token> tokens)
        {
            _head = _headPos == -1 ? this : tokens[_headPos];
        }
    }
}
