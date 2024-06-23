namespace BebiLibs.Analytics
{
    public abstract class ConstantString
    {
        protected string _stringValue;

        internal ConstantString()
        {
            _stringValue = string.Empty;
        }

        internal ConstantString(string stringValue)
        {
            _stringValue = stringValue;
        }

        public static implicit operator string(ConstantString d) => d._stringValue;
        public override string ToString() => _stringValue;
    }

}
