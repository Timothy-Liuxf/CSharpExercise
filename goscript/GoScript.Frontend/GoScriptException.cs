using GoScript.Frontend.Types;
using GoScript.Utils;

namespace GoScript.Frontend
{
    public class GoScriptException : Exception
    {
        public GoScriptException() { }
        public GoScriptException(string message) : base(message) { }
    }

    public class InternalErrorException : GoScriptException
    {
        public InternalErrorException(string message) : base($"Internal Error: {message}") { }
    }

    public abstract class CodeErrorException : GoScriptException
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

    public class LanguageFeatureException : GoScriptException
    {
    }

    public class BreakException : LanguageFeatureException
    {
    }

    public class ContinueException : LanguageFeatureException
    {
    }

    public class ReturnException : LanguageFeatureException
    {
        public IReadOnlyList<object?> ReturnValues { get; private init; }

        public ReturnException(IEnumerable<object?> returnValues)
        {
            this.ReturnValues = returnValues.ToList();
        }
    }

    // public class ReturnException : LanguageFeatureException
    // {
    //     public object ReturnValue { get; private init; }
    //     public ReturnException(object returnValue) => this.ReturnValue = returnValue;
    // }
}
