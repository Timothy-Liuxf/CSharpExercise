namespace JsonUtils.Frontend.AST
{
    internal class StringValue : JsonObject
    {
        public string Value { get; init; }
        public SourceLocation Location { get; init; }

        public override string ToString()
        {
            return $"\"{Value}\"";
        }

        public StringValue(string value, SourceLocation location)
        {
            Value = value;
            Location = location;
        }
    }
}
