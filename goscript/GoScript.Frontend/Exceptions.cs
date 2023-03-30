using GoScript.Frontend.Types;
using GoScript.Utils;

namespace GoScript.Frontend
{
    public class InternalErrorException : Exception
    {
        public InternalErrorException(string message) : base($"Internal Error: {message}") { }
    }

    public abstract class CodeErrorException : Exception
    {
        public CodeErrorException(string message) : base(message) { }
    }

    public class SyntaxErrorException : CodeErrorException
    {
        public SyntaxErrorException(SourceLocation location, string message)
            : base($"Syntax error at: {location}: {message}") { }

        public SyntaxErrorException(string message) : base($"Syntax error: {message}") { }
    }

    public class ConflictException : CodeErrorException
    {
        public ConflictException(string message) : base(message) { }
    }

    public class SymbolNotFoundException : CodeErrorException
    {
        public SymbolNotFoundException(string message) : base(message) { }
    }

    public class InvalidOperationException : CodeErrorException
    {
        public InvalidOperationException(string message) : base(message) { }
    }

    public class TypeErrorException : CodeErrorException
    {
        public TypeErrorException(GSType expected, GSType actual, SourceLocation location)
            : base($"Type error at {location}: Expected {expected}, found {actual}.") { }
    }
}
