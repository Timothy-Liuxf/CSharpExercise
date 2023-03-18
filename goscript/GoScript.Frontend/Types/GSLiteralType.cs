namespace GoScript.Frontend.Types
{
    public abstract class GSLiteralType : GSType
    {
        public override bool IsLiteral => true;
    }

    public sealed class GSIntegerLiteral : GSLiteralType
    {
        public override bool IsIntegerLiteral => true;

        public static GSIntegerLiteral Instance { get; } = new();

        private GSIntegerLiteral() { }
    }
}
