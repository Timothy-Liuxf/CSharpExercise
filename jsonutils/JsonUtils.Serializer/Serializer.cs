namespace JsonUtils.Serializer
{
    public static class Serializer
    {
        public static T Deserialize<T>(TextReader reader)
        {
            var ast = new Frontend.FrontEnd(reader).Parse();
            return new DeserializeHelper<T>(ast).Deserialize();
        }

        public static string Serialize(object? obj, bool ensureASCII = true)
        {
            return new SerializeHelper(obj, ensureASCII).Serialize();
        }
    }
}
