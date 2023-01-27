using JsonUtils.Frontend;
using JsonUtils.Utils;
using System.Collections;
using System.Reflection;
using System.Text;

namespace JsonUtils.Serializer
{
    internal class SerializeHelper
    {
        private StringBuilder stringBuilder = new();
        private bool finish = false;
        private object? obj;

        public string Serialize()
        {
            if (!finish)
            {
                SerializeImpl(this.obj);
                finish = true;
            }
            return stringBuilder.ToString();
        }

        private void SerializeImpl(object? obj)
        {
            if (obj is null)
            {
                this.stringBuilder.Append(Token.NullLiteral);
                return;
            }

            var type = obj.GetType();
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type.IsArray)
            {
                stringBuilder.Append("[");
                foreach (var elem in (IEnumerable)obj)
                {
                    SerializeImpl(elem);
                    stringBuilder.Append(",");
                }
                stringBuilder.Append("]");
            }
            else if (type.IsEnum)
            {
                stringBuilder.Append(Convert.ToInt64(obj).ToString());
            }
            else if (type.IsBoolean())
            {
                stringBuilder.Append((bool)obj ? Token.TrueLiteral : Token.FalseLiteral);
            }
            else if (type.IsNumber())
            {
                stringBuilder.Append(obj.ToString());
            }
            else if (type.IsString())
            {
                SerializeImpl((string)obj);
            }
            else
            {
                stringBuilder.Append("{");
                var properties = type.GetProperties();
                foreach (var property in properties)
                {
                    var attrs = property.GetCustomAttributes<JsonSerializeOptionAttribute>();
                    var attrCount = attrs.Count();
                    if (attrCount == 0) continue;
                    if (attrCount > 1)
                    {
                        throw new DuplicateAttributeException($"Duplicate attribute {nameof(JsonSerializeOptionAttribute)} on property {property.Name}.");
                    }

                    var attr = attrs.First();
                    SerializeImpl(attr.Key);
                    stringBuilder.Append(":");
                    SerializeImpl(property.GetValue(obj));
                    stringBuilder.Append(",");
                }
                stringBuilder.Append("}");
            }
        }

        private void SerializeImpl(string @string)
        {
            stringBuilder.Append($"\"{@string}\"");
        }

        public SerializeHelper(object? obj)
        {
            this.obj = obj;
        }
    }
}
