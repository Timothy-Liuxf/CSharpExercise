namespace GoScript.Frontend.Types
{
    public abstract class GSConstantType : GSType
    {
        public override bool IsConstant => true;

        public override bool EqualsImpl(GSType other)
        {
            return this.GetType() == other.GetType();
        }

        public override int GetHashCodeImpl()
        {
            return this.GetType().GetHashCode();
        }
    }

    public sealed class GSIntegerConstant : GSConstantType
    {
        public override bool IsIntegerConstant => true;

        public static GSIntegerConstant Instance { get; } = new();

        private GSIntegerConstant() { }

        public override string ToString() => "integer constant";
    }

    public sealed class GSBoolConstant : GSConstantType
    {
        public override bool IsBoolConstant => true;

        public static GSBoolConstant Instance { get; } = new();

        private GSBoolConstant() { }

        public override string ToString() => "bool constant";
    }
}
