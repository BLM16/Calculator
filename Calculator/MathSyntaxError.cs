namespace Calculator
{
    public class MathSyntaxError : System.ApplicationException
    {
        public MathSyntaxError(string message) : base(message) { }
        public MathSyntaxError(string message, System.Exception innerException) : base(message, innerException) { }
    }
}
