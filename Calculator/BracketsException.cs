namespace Calculator
{
    public class BracketsException : System.ApplicationException
    {
        public BracketsException(string message) : base(message) { }
        public BracketsException(string message, System.Exception innerException) : base(message, innerException) { }
    }
}
