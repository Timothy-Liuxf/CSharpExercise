using JsonUtils.Frontend;
using System.Reflection;

namespace JsonUtils.Serializer
{
    public static class Serializer
    {
        public static T Deserialize<T>(StreamReader reader)
        {
            var ast = new Frontend.FrontEnd(reader).Parse();
            Activator.CreateInstance(typeof(T));
            return default(T);
        }
    }
}
