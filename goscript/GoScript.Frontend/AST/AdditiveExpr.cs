using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public sealed class AdditiveExpr : ArithmeticExpression
    {
        public enum OperatorType
        {
            Add,
            Sub,
        }

        public OperatorType Operator { get; private init; }

        public AdditiveExpr(Expression lExpr, Expression rExpr, OperatorType @operator, SourceLocation location)
            : base(lExpr, rExpr, location)
        {
            this.Operator = @operator;
        }

        public override string ToString()
        {
            var ch = this.Operator switch
            {
                OperatorType.Add => '+',
                OperatorType.Sub => '-',
                _ => throw new InternalErrorException("Unknown operator type."),
            };
            return $"{this.LExpr} {ch} {this.RExpr}";
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
