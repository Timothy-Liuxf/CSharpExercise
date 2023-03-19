namespace GoScript.Frontend.Types
{
    public abstract class GSType
    {
        public static bool operator ==(GSType type1, GSType type2)
        {
            if (type1.IsBasic)
            {
                return (type2.IsBasic) && (((GSBasicType)type1).DotNetType == ((GSBasicType)type2).DotNetType);
            }
            else
            {
                return type1.GetType() == type2.GetType();
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

        public virtual bool IsBasic => false;
        public virtual bool IsArithmetic => false;
        public virtual bool IsBool => false;
        public virtual bool IsLiteral => false;
        public virtual bool IsIntegerLiteral => false;
        public virtual bool IsBoolLiteral => false;
    }
}
