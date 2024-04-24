using System.Collections.Generic;
using System.Numerics;
using Python.Runtime;
using SpacyDotNet;

namespace SpaCyDotNet.api
{
    public class StringStore
    {
        private readonly dynamic _pyStringStore;

        private readonly Dictionary<string, BigInteger> _dictStrToNumber;
        private readonly Dictionary<BigInteger, string> _dictNumberToStr;

        internal StringStore(dynamic stringStore)
        {
            _pyStringStore = stringStore;
            _dictStrToNumber = new Dictionary<string, BigInteger>();
            _dictNumberToStr = new Dictionary<BigInteger, string>();            
        }

        public object this[object key]
        {
            get
            {
                if (key is string keyStr)
                {                    
                    if (_dictStrToNumber.TryGetValue(keyStr, value: out var item1))
                        return item1;

                    BigInteger valHash;
                    using (Py.GIL())
                    {
                        var dynPyNumber = _pyStringStore.__getitem__(key);
                        valHash = new PyInt(dynPyNumber).ToBigInteger();
                        _dictStrToNumber.Add(keyStr, valHash);
                    }

                    return valHash;
                }

                var keyHash = key.AsBigInteger();
                if (_dictNumberToStr.TryGetValue(keyHash, out var item))
                    return item;

                string valStr;
                using (Py.GIL())
                {
                    var dynPyStr = _pyStringStore.__getitem__(key);
                    var pyString = new PyString(dynPyStr);
                    valStr = pyString.ToString();
                    _dictNumberToStr.Add(keyHash, valStr);
                }

                return valStr;
            }
        }
    }
}
