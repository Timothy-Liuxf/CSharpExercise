namespace JsonUtils.Serializer
{
    public class JsonSerializeOptionAttribute : Attribute
    {
        internal string Key { get; init; }
        internal bool Required { get; init; }
        internal object? DefaultValue { get; init; }

        public JsonSerializeOptionAttribute(string key, bool required, object? defaultValue = null)
        {
            Key = key;
            Required = required;
            DefaultValue = defaultValue;
        }
    }
}
