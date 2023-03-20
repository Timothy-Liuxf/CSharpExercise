using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public sealed class AdditiveExpr : Expression
    {
        public enum OperatorType
        {
            Add,
            Sub,
        }

        public Expression LExpr { get; private init; }
        public Expression RExpr { get; private init; }
        public OperatorType Operator { get; private init; }

        public AdditiveExpr(Expression lExpr, Expression rExpr, OperatorType @operator, SourceLocation location)
            : base(location)
        {
            this.LExpr = lExpr;
            this.RExpr = rExpr;
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
