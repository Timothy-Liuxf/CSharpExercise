namespace GoScript.Frontend.Types
{
    public abstract class GSType
    {
        public static bool operator ==(GSType type1, GSType type2)
        {
            return type1.EqualsImpl(type2);
        }

        public static bool operator !=(GSType type1, GSType type2)
        {
            return !(type1 == type2);
        }

        public override bool Equals(object? obj)
        {
            return (obj is GSType) && this.EqualsImpl((GSType)obj);
        }

        public abstract bool EqualsImpl(GSType other);

        public override int GetHashCode()
        {
            return this.GetHashCodeImpl();
        }
        public abstract int GetHashCodeImpl();

        public virtual bool IsBasic => false;
        public virtual bool IsArithmetic => false;
        public virtual bool IsBool => false;
        public virtual bool IsConstant => false;
        public virtual bool IsIntegerConstant => false;
        public virtual bool IsBoolConstant => false;
        public virtual bool IsFunc => false;
    }
}
