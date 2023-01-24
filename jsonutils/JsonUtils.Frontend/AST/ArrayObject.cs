namespace JsonUtils.Frontend.AST
{
    public sealed class ArrayObject : JsonObject
    {
        private IList<JsonObject> objects;
        public IEnumerable<JsonObject> Objects => objects;
        public SourceLocation Location { get; init; }

        public override string ToString()
        {
            string ret = "[ ";
            foreach (var obj in objects)
            {
                ret += obj.ToString() + ", ";
            }
            return ret + "]";
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public ArrayObject(IList<JsonObject> objects, SourceLocation location)
        {
            this.objects = objects;
            Location = location;
        }
    }
}
