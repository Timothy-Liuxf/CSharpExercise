namespace GoScript.Frontend.Types
{
    public abstract class GSConstantType : GSType
    {
        public override bool IsConstant => true;
    }

    public sealed class GSIntegerConstant : GSConstantType
    {
        public override bool IsIntegerConstant => true;

        public static GSIntegerConstant Instance { get; } = new();

        private GSIntegerConstant() { }
    }

    public sealed class GSBoolConstant : GSConstantType
    {
        public override bool IsBoolConstant => true;

        public static GSBoolConstant Instance { get; } = new();

        private GSBoolConstant() { }
    }
}
