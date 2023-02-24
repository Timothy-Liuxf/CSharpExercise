using GoScript.Utils;

namespace GoScript.Frontend
{
    public class InternalErrorException : Exception
    {
        public InternalErrorException(string message) : base($"Internal Error: {message}") { }
    }

    public class SyntaxErrorException : Exception
    {
        public SyntaxErrorException(SourceLocation location, string message)
            : base($"Syntax error at: {location}: {message}") { }
    }
}
