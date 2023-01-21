namespace JsonUtils.Frontend.AST
{
    public class ArrayObject : JsonObject
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

        public ArrayObject(IList<JsonObject> objects)
        {
            this.objects = objects;
        }
    }
}
