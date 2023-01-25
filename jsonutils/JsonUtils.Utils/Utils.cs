namespace JsonUtils.Utils
{
    public static class Tools
    {
        public static U CreateInstance<U>()
        {
            return (U)(Activator.CreateInstance(typeof(U))
                ?? throw new SystemException($"Failed to create an instance of type {typeof(U).FullName}."));
        }

        public static object CreateInstance(Type type)
        {
            return Activator.CreateInstance(type)
                ?? throw new SystemException($"Failed to create an instance of type {type.FullName}.");
        }

        public static Array CreateArrayInstance(Type elementType, int length)
        {
            return Array.CreateInstance(elementType, length)
                ?? throw new SystemException($"Failed to create an array instance of type {elementType.FullName}.");
        }

        public static bool IsNumber(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
            }
            if (type == typeof(nint) || type == typeof(nuint))
            {
                return true;
            }
            return false;
        }

        public static bool IsFloatingPoint(this Type type)
        {
            var typeCode = Type.GetTypeCode(type);
            return typeCode == TypeCode.Double || typeCode == TypeCode.Single;
        }

        public static bool IsDecimal(this Type type)
        {
            return Type.GetTypeCode(type) == TypeCode.Decimal;
        }

        public static bool IsInteger(this Type type)
        {
            return type.IsNumber() && !type.IsFloatingPoint() && !type.IsDecimal();
        }

        public static bool IsString(this Type type)
        {
            return Type.GetTypeCode(type) == TypeCode.String;
        }

        public static bool IsChar(this Type type)
        {
            return Type.GetTypeCode(type) == TypeCode.Char;
        }

        public static bool IsBoolean(this Type type)
        {
            return Type.GetTypeCode(type) == TypeCode.Boolean;
        }
    }
}
