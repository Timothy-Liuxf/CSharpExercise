﻿namespace JsonUtils.Frontend.AST
{
    public sealed class ClassObject : JsonObject
    {
        private IDictionary<string, (SourceLocation, JsonObject)> properties;

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

        public ClassObject(IDictionary<string, (SourceLocation, JsonObject)> properties)
        {
            this.properties = properties;
        }
    }
}
