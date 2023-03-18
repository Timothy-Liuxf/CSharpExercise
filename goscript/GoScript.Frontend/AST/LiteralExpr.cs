using GoScript.Frontend.Types;

namespace GoScript.Frontend.AST
{
    public abstract class LiteralExpr : Expression
    {
    }

    public sealed class IntegerLiteralExpr : LiteralExpr
    {
        public ulong IntegerValue { get; private init; }
        public IntegerLiteralExpr(ulong integerValue)
        {
            this.IntegerValue = integerValue;
        }

        public override string ToString()
        {
            return this.IntegerValue.ToString();
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
