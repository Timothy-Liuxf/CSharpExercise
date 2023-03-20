using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public abstract class ConstantExpr : Expression
    {
        public ConstantExpr(SourceLocation location) : base(location)
        {
        }
    }

    public sealed class IntegerConstantExpr : ConstantExpr
    {
        public ulong IntegerValue { get; private init; }
        public IntegerConstantExpr(ulong integerValue, SourceLocation location) : base(location)
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
        public BoolConstantExpr(bool boolValue, SourceLocation location) : base(location)
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
