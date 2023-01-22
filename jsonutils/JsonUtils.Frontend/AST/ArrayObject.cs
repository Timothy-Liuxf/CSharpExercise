namespace JsonUtils.Frontend.AST
{
    public sealed class ArrayObject : JsonObject
    {
        private IList<JsonObject> objects;

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

        public ArrayObject(IList<JsonObject> objects)
        {
            this.objects = objects;
        }
    }
}
