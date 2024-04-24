using Python.Runtime;
using SpacyDotNet;

namespace SpaCyDotNet.api
{
    public class Displacy
    {
        public static void Serve(Doc doc, string style)
        {
            using (Py.GIL())
            {
                dynamic spacy = Py.Import("spacy");

                var pyDoc = doc.PyDoc;
                var pyStyle = new PyString(style);
                spacy.displacy.serve(pyDoc, pyStyle);
            }
        }
    }
}
