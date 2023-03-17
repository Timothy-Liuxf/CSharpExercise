using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public sealed class UnaryExpr : Expression
    {
        public Expression Operand { get; private init; }
        public SourceLocation Location { get; private init; }

        public UnaryExpr(Expression operand, SourceLocation location)
        {
            this.Operand = operand;
            this.Location = location;
        }

        public override string ToString()
        {
            return $"-{this.Operand}";
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
