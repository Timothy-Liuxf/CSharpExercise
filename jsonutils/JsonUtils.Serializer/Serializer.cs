using JsonUtils.Frontend;
using System.Reflection;

namespace JsonUtils.Serializer
{
    public static class Serializer
    {
        public static T Deserialize<T>(StreamReader reader)
            where T : notnull
        {
            var ast = new Frontend.FrontEnd(reader).Parse();
            return new DeserializeHelper<T>(ast).Deserialize();
        }
    }
}
