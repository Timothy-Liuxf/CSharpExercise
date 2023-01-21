namespace JsonUtils.Frontend.AST
{
    public class BooleanValue : JsonObject
    {
        public bool Value { get; init; }
        public SourceLocation Location { get; init; }

        public override string ToString()
        {
            return Value ? "true" : "false";
        }

        public BooleanValue(bool value, SourceLocation location)
        {
            Value = value;
            Location = location;
        }
    }
}
