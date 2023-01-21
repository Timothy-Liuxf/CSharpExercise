namespace JsonUtils.Frontend
{
    public class SyntaxErrorException : Exception
    {
        public SyntaxErrorException(SourceLocation location, string message) : base($"Syntax error at ({location.Line}, {location.Column}): {message}") { }
    }

    public class IntegerOverflowException : OverflowException
    {
        public IntegerOverflowException(SourceLocation location, string message) : base($"Integer overlow at ({location.Line}, {location.Column}): {message}") { }
    }

    public class DuplicatedKeyException : Exception
    {
        public DuplicatedKeyException(SourceLocation location, string key, SourceLocation previousKeyLocation)
            : base($"Duplicated key \"{key}\" at ({location.Line}, {location.Column}): "
                  + $"Already defined at ({previousKeyLocation.Line}, {previousKeyLocation.Column})")
        { }
    }
}
