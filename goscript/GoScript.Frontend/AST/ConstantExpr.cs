using GoScript.Frontend.Types;

namespace GoScript.Frontend.AST
{
    public abstract class ConstantExpr : Expression
    {
    }

    public sealed class IntegerConstantExpr : ConstantExpr
    {
        public ulong IntegerValue { get; private init; }
        public IntegerConstantExpr(ulong integerValue)
        {
            this.IntegerValue = integerValue;
        }

        public override string ToString()
        {
            return this.IntegerValue.ToString();
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }

    public sealed class BoolConstantExpr : ConstantExpr
    {
        public bool BoolValue { get; private init; }
        public BoolConstantExpr(bool boolValue)
        {
            this.BoolValue = boolValue;
        }

        public override string ToString()
        {
            return this.BoolValue ? "true" : "false";
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
