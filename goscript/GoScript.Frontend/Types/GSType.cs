namespace GoScript.Frontend.Types
{
    public abstract class GSType
    {
        public static bool operator ==(GSType type1, GSType type2)
        {
            if (type1 is GSBasicType)
            {
                return (type2 is GSBasicType) && (((GSBasicType)type1).DotNetType == ((GSBasicType)type2).DotNetType);
            }
            else
            {
                return type1 is GSNilType && type2 is GSNilType;
            }
        }

        public static bool operator !=(GSType type1, GSType type2)
        {
            return !(type1 == type2);
        }

        public override bool Equals(object? obj)
        {
            return (obj is GSType) && (this == (GSType)obj);
        }

        public override int GetHashCode()
        {
            return (this is GSBasicType) ? ((GSBasicType)this).DotNetType.GetHashCode() : 0;
        }
    }
}
