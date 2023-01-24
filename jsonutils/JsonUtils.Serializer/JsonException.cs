namespace JsonUtils.Serializer
{
    public class DuplicateAttributeException : Exception
    {
        public DuplicateAttributeException(string message) : base(message) { }
    }

    public class JsonKeyNotExistException : Exception
    {
        public JsonKeyNotExistException(string message) : base(message) { }
    }

    public class TypeErrorException : Exception
    {
        public TypeErrorException(string message) : base(message) { }
    }
}
