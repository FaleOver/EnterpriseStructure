namespace Common
{
    public class BusinessValidationException : Exception
    {
        public BusinessValidationException(string message) : base(message) { }
    }
}
