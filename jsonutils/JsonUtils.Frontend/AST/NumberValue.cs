namespace JsonUtils.Frontend.AST
{
    public class NumberValue : JsonObject
    {
        public int Value { get; init; }
        public SourceLocation Location { get; init; }

        public override string ToString()
        {
            return Value.ToString();
        }

        public NumberValue(int value, SourceLocation location)
        {
            Value = value;
            Location = location;
        }
    }
}
