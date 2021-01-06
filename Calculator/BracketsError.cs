namespace Calculator
{
    public class BracketsError : System.ApplicationException
    {
        public BracketsError(string message) : base(message) { }
        public BracketsError(string message, System.Exception innerException) : base(message, innerException) { }
    }
}
