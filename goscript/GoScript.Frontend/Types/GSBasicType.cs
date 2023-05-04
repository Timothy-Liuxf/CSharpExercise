namespace GoScript.Frontend.Types
{
    public abstract class GSBasicType : GSType
    {
        public abstract Type DotNetType { get; }

        public override bool IsBasic => true;

        public static GSBasicType? ParseBasicType(string type)
        {
            return type switch
            {
                "int16" => GSInt16.Instance,
                "int32" => GSInt32.Instance,
                "int" or "int64" => GSInt64.Instance,
                "uint16" => GSUInt16.Instance,
                "uint32" => GSUInt32.Instance,
                "uint" or "uint64" => GSUInt64.Instance,
                "bool" => GSBool.Instance,
                _ => null,
            };
        }

        public override bool EqualsImpl(GSType other)
        {
            return other.IsBasic && (this.DotNetType == (other as GSBasicType)!.DotNetType);
        }

        public override int GetHashCodeImpl()
        {
            return this.DotNetType.GetHashCode();
        }

        // public static GSBasicType GetCommonType(GSBasicType type1, GSBasicType type2)
        // {
        //     return type1 switch
        //     {
        //         GSInt16 => type2 switch
        //         {
        //             GSInt16 => new GSInt16(),
        //             GSInt32 => new GSInt32(),
        //             GSInt64 => new GSInt64(),
        //             _ => throw new InternalErrorException($"Type {type2} is not a basic type."),
        //         },
        //         GSInt32 => type2 switch
        //         {
        //             GSInt16 => new GSInt32(),
        //             GSInt32 => new GSInt32(),
        //             GSInt64 => new GSInt64(),
        //             _ => throw new InternalErrorException($"Type {type2} is not a basic type."),
        //         },
        //         GSInt64 => type2 switch
        //         {
        //             GSInt16 => new GSInt64(),
        //             GSInt32 => new GSInt64(),
        //             GSInt64 => new GSInt64(),
        //             _ => throw new InternalErrorException($"Type {type2} is not a basic type."),
        //         },
        //         _ => throw new InternalErrorException($"Type {type1} is not a basic type."),
        //     };
        // }
    }

    public abstract class GSArithmeticType : GSBasicType
    {
        public override bool IsArithmetic => true;
        public abstract bool IsSigned { get; }
    }

    public sealed class GSInt16 : GSArithmeticType
    {
        public override Type DotNetType => typeof(short);

        public override string ToString() => "int16";

        public override bool IsSigned => true;

        public static GSInt16 Instance { get; } = new();

        private GSInt16() { }
    }

    public sealed class GSInt32 : GSArithmeticType
    {
        public override Type DotNetType => typeof(int);

        public override string ToString() => "int32";

        public override bool IsSigned => true;

        public static GSInt32 Instance { get; } = new();

        private GSInt32() { }
    }

    public sealed class GSInt64 : GSArithmeticType
    {
        public override Type DotNetType => typeof(long);

        public override string ToString() => "int64";

        public override bool IsSigned => true;

        public static GSInt64 Instance { get; } = new();

        private GSInt64() { }
    }

    public sealed class GSUInt16 : GSArithmeticType
    {
        public override Type DotNetType => typeof(ushort);

        public override string ToString() => "uint16";

        public override bool IsSigned => false;

        public static GSUInt16 Instance { get; } = new();

        private GSUInt16() { }
    }

    public sealed class GSUInt32 : GSArithmeticType
    {
        public override Type DotNetType => typeof(uint);

        public override string ToString() => "uint32";

        public override bool IsSigned => false;

        public static GSUInt32 Instance { get; } = new();

        private GSUInt32() { }
    }

    public sealed class GSUInt64 : GSArithmeticType
    {
        public override Type DotNetType => typeof(ulong);

        public override string ToString() => "uint64";

        public override bool IsSigned => false;

        public static GSUInt64 Instance { get; } = new();

        private GSUInt64() { }
    }

    public sealed class GSBool : GSBasicType
    {
        public override Type DotNetType => typeof(bool);

        public override string ToString() => "bool";

        public override bool IsBool => true;

        public static GSBool Instance { get; } = new();

        private GSBool() { }

    }
}
