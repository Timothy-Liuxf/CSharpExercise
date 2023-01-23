namespace JsonUtils.Frontend.AST
{
    public class NullValue : JsonObject
    {
        public SourceLocation Location { get; init; }

        public override string ToString()
        {
            return "null";
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public NullValue(SourceLocation location)
        {
            Location = location;
        }
    }
}
