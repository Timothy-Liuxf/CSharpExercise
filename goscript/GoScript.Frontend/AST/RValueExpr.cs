using GoScript.Frontend.Types;

namespace GoScript.Frontend.AST
{
    public abstract class RValueExpr : Expression
    {
    }

    public sealed class IntegerRValueExpr : RValueExpr
    {
        public ulong IntegerValue { get; private init; }
        public IntegerRValueExpr(ulong integerValue)
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
