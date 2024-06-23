using System;

namespace BebiLibs
{
    [Serializable]
    public class StringTuple
    {
        public string Key;
        public string Value;

        public StringTuple(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public StringTuple Clone()
        {
            return new StringTuple(Key, Value);
        }

        public override string ToString()
        {
            return $"Key: {Key}, Value: {Value}";
        }
    }
}
