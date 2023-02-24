namespace GoScript.Frontend
{
    public class InternalErrorException : Exception
    {
        private string message;

        public override string Message => $"Internal Error: {this.message}";

        public InternalErrorException(string message)
        {
            this.message = message;
        }
    }
}
