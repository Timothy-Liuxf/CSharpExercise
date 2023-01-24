using JsonUtils.Frontend;
using JsonUtils.Frontend.AST;
using JsonUtils.Utils;
using System.Globalization;
using System.Reflection;

namespace JsonUtils.Serializer
{
    internal class DeserializeHelper<T> : IVisitor
        where T : notnull
    {

        private object deserialized;
        private ASTNode ast;
        private bool finished = false;

        public T Deserialize()
        {
            return (T)(finished ? deserialized : DeserializeOnce());
        }

        private object DeserializeOnce()
        {
            this.ast.Accept(this);
            finished = true;
            return this.deserialized;
        }

        public void Visit(ClassObject obj)
        {
            var type = (Type)this.deserialized;
            if (type.IsNumber() || type.IsChar() || type.IsString() || type.IsBoolean() || type.IsEnum)
            {
                throw new TypeErrorException($"Expect object type at ({obj.Location.Line}, {obj.Location.Column}), found {type.FullName}.");
            }

            var deserialized = Tools.CreateInstance(type);
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var attrs = property.GetCustomAttributes<JsonSerializeOptionAttribute>().GetEnumerator();
                if (!attrs.MoveNext()) continue;
                var attr = attrs.Current;
                if (attrs.MoveNext())
                {
                    throw new DuplicateAttributeException($"Duplicate attribute {nameof(JsonSerializeOptionAttribute)} on property {property.Name}.");
                }

                var jsonProperties = obj.Properties;
                if (!jsonProperties.ContainsKey(attr.Key))
                {
                    if (attr.Required)
                    {
                        throw new JsonKeyNotExistException($"Json key {attr.Key} doesn't exist but is required.");
                    }
                    else
                    {
                        property.SetValue(deserialized, attr.DefaultValue);
                    }
                }
                else
                {
                    this.deserialized = property.PropertyType;
                    jsonProperties[attr.Key].Item2.Accept(this);
                    property.SetValue(deserialized, this.deserialized);
                }
            }
            this.deserialized = deserialized;
        }

        public void Visit(ArrayObject array)
        {
            var type = (Type)this.deserialized;
            if (!type.IsArray)
            {
                throw new TypeErrorException($"Expected array type at ({array.Location.Line}, {array.Location.Column}), found type {type.FullName}.");
            }

            var elementType = type.GetElementType()!;
            var deserialized = Tools.CreateArrayInstance(elementType, array.Objects.Count());
            foreach (var (arrayElem, i) in array.Objects.Select((val, i) => (val, i)))
            {
                this.deserialized = elementType;
                arrayElem.Accept(this);
                deserialized.SetValue(this.deserialized, i);
            }
            this.deserialized = deserialized;
        }

        public void Visit(BooleanValue boolean)
        {
            var type = (Type)this.deserialized;
            if (!type.IsBoolean())
            {
                throw new TypeErrorException($"Expected boolean type at ({boolean.Location.Line}, {boolean.Location.Column}), found type {type.FullName}.");
            }
            this.deserialized = boolean.Value;
        }

        public void Visit(StringValue @string)
        {
            var type = (Type)this.deserialized;
            if (!type.IsString())
            {
                throw new TypeErrorException($"Expected string type at ({@string.Location.Line}, {@string.Location.Column}), found type {type.FullName}.");
            }
            this.deserialized = @string.Value;
        }

        public void Visit(NullValue @null)
        {
            var type = (Type)this.deserialized;
            if (type.IsValueType && Nullable.GetUnderlyingType(type) is null)
            {
                throw new TypeErrorException($"Expected nullable type at ({@null.Location.Line}, {@null.Location.Column}), found type {type.FullName}.");
            }
            this.deserialized = null!;
        }

        public void Visit(NumberValue number)
        {
            var type = (Type)this.deserialized;
            string numberFormat = number.Value;
            bool isHex = false, isScience = false, isFloatingPoint = false, isInteger = false;
            _ = isInteger;
            if (numberFormat.StartsWith("0x") || numberFormat.StartsWith("0X"))
            {
                isHex = true;
            }
            else if (numberFormat.IndexOfAny("eE".ToCharArray()) != -1)
            {
                isScience = true;
            }
            else if (numberFormat.Contains('.'))
            {
                isFloatingPoint = true;
            }
            else
            {
                isInteger = true;
            }

            try
            {
                if (type.IsEnum)
                {
                    if (isScience || isFloatingPoint)
                    {
                        throw new TypeErrorException($"Number {numberFormat} cannot be parsed as enum type {type.FullName}.");
                    }
                    else
                    {
                        this.deserialized = Enum.Parse(type, isHex ? long.Parse(numberFormat, NumberStyles.HexNumber).ToString() : numberFormat);
                    }
                }
                else
                {
                    if (!type.IsNumber())
                    {
                        throw new TypeErrorException($"Expected numeric type at ({number.Location.Line}, {number.Location.Column}) when parsing {numberFormat}, found type {type.FullName}.");
                    }

                    if (type.IsInteger())
                    {
                        if (isFloatingPoint || isScience)
                        {
                            throw new TypeErrorException($"Expected floating point type at ({number.Location.Line}, {number.Location.Column}) when parsing {numberFormat}, found type {type.FullName}.");
                        }
                        var parseMethod = type.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static,
                            isHex ? new Type[] { typeof(string), typeof(NumberStyles) } : new Type[] { typeof(string) })!;
                        this.deserialized = parseMethod.Invoke(null,
                            isHex ? new object[] { numberFormat, NumberStyles.HexNumber } : new object[] { numberFormat })!;
                    }
                    else
                    {
                        var parseMethod = type.GetMethod("Parse",
                            BindingFlags.Public | BindingFlags.Static, new Type[] { typeof(string) })!;
                        if (isHex)
                        {
                            var decString = long.Parse(numberFormat, NumberStyles.HexNumber);
                            this.deserialized = parseMethod.Invoke(null, new object[] { decString })!;
                        }
                        else
                        {
                            this.deserialized = parseMethod.Invoke(null, new object[] { numberFormat })!;
                        }
                    }
                }
            }
            catch (FormatException ex)
            {
                throw new SyntaxErrorException(number.Location, $"Error when parsing {numberFormat}: {ex.ToString()}");
            }
            catch (OverflowException ex)
            {
                throw new IntegerOverflowException(number.Location, $"Error when parsing {numberFormat}: {ex.ToString()}");
            }
        }

        public DeserializeHelper(ASTNode ast)
        {
            this.deserialized = typeof(T);
            this.ast = ast;
        }
    }
}
