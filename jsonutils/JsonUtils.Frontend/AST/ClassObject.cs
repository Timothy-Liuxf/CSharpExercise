namespace JsonUtils.Frontend.AST
{
    public sealed class ClassObject : JsonObject
    {
        private IReadOnlyDictionary<string, (SourceLocation, JsonObject)> properties;
        public IReadOnlyDictionary<string, (SourceLocation, JsonObject)> Properties => properties;
        public SourceLocation Location { get; init; }

        public override string ToString()
        {
            string ret = "{ ";
            foreach (var (key, (_, val)) in properties)
            {
                ret += $"\"{key}\": {val.ToString()}, ";
            }
            return ret + "}";
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public ClassObject(IReadOnlyDictionary<string, (SourceLocation, JsonObject)> properties, SourceLocation location)
        {
            this.properties = properties;
            Location = location;
        }
    }
}
