using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public sealed class UnaryExpr : Expression
    {
        public enum OperatorType
        {
            Neg,
            Not,
        }

        public OperatorType Operator { get; private init; }
        public Expression Operand { get; private init; }

        public UnaryExpr(OperatorType @operator, Expression operand, SourceLocation location)
            : base(location)
        {
            this.Operator = @operator;
            this.Operand = operand;
        }

        public override string ToString()
        {
            return $"-{this.Operand}";
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
