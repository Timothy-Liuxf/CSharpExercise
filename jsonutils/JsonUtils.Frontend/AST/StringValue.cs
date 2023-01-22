namespace JsonUtils.Frontend.AST
{
    public sealed class StringValue : JsonObject
    {
        public string Value { get; init; }
        public SourceLocation Location { get; init; }

        public override string ToString()
        {
            return $"\"{Value}\"";
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public StringValue(string value, SourceLocation location)
        {
            Value = value;
            Location = location;
        }
    }
}
