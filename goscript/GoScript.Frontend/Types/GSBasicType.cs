﻿namespace GoScript.Frontend.Types
{
    public abstract class GSBasicType : GSType
    {
        public abstract Type DotNetType { get; }

        public static GSBasicType? ParseBasicType(string type)
        {
            return type switch
            {
                "int16" => new GSInt16(),
                "int" or "int32" => new GSInt32(),
                "int64" => new GSInt64(),
                _ => null,
            };
        }

        public static GSBasicType GetCommonType(GSBasicType type1, GSBasicType type2)
        {
            return type1 switch
            {
                GSInt16 => type2 switch
                {
                    GSInt16 => new GSInt16(),
                    GSInt32 => new GSInt32(),
                    GSInt64 => new GSInt64(),
                    _ => throw new InternalErrorException($"Type {type2} is not a basic type."),
                },
                GSInt32 => type2 switch
                {
                    GSInt16 => new GSInt32(),
                    GSInt32 => new GSInt32(),
                    GSInt64 => new GSInt64(),
                    _ => throw new InternalErrorException($"Type {type2} is not a basic type."),
                },
                GSInt64 => type2 switch
                {
                    GSInt16 => new GSInt64(),
                    GSInt32 => new GSInt64(),
                    GSInt64 => new GSInt64(),
                    _ => throw new InternalErrorException($"Type {type2} is not a basic type."),
                },
                _ => throw new InternalErrorException($"Type {type1} is not a basic type."),
            };
        }
    }

    public sealed class GSInt16 : GSBasicType
    {
        public override Type DotNetType => typeof(short);
    }

    public sealed class GSInt32 : GSBasicType
    {
        public override Type DotNetType => typeof(int);
    }

    public sealed class GSInt64 : GSBasicType
    {
        public override Type DotNetType => typeof(long);
    }
}
