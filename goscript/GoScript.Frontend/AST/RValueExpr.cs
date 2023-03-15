using GoScript.Frontend.Types;

namespace GoScript.Frontend.AST
{
    public abstract class RValueExpr : Expression
    {
        public override object? Value
        {
            internal set => throw new InternalErrorException("RValueExpr cannot set type.");
        }
    }

    public sealed class IntegerRValueExpr : RValueExpr
    {
        public override object? Value
        {
            get => this.IntegerValue;
        }

        public override GSType? ExprType { get; internal set; }

        public ulong IntegerValue { get; private init; }
        public IntegerRValueExpr(ulong integerValue)
        {
            this.IntegerValue = integerValue;
        }

        public override string ToString()
        {
            return this.IntegerValue.ToString();
        }
    }
}
