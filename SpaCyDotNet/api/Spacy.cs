using Python.Runtime;

namespace SpaCyDotNet.api
{
    public class Spacy
    {
        public static Lang Load(string model)
        {
            using (Py.GIL())
            {
                dynamic spacy = Py.Import("spacy");
                var pyString = new PyString(model);
                var nlp = spacy.load(pyString);
                return  new Lang(nlp);
            }
        }
    }
}
