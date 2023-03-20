using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public sealed class ComparisonExpr : ArithmeticExpression
    {
        public enum OperatorType
        {
            Equ,
            Neq,
            Gre,
            Les,
            Geq,
            Leq,
        }

        public OperatorType Operator { get; private init; }

        public ComparisonExpr(Expression lExpr, Expression rExpr, OperatorType @operator, SourceLocation location)
            : base(lExpr, rExpr, location)
        {
            this.Operator = @operator;
        }

        public override string ToString()
        {
            var ch = this.Operator switch
            {
                OperatorType.Equ => "==",
                OperatorType.Neq => "!=",
                OperatorType.Gre => ">",
                OperatorType.Les => "<",
                OperatorType.Geq => ">=",
                OperatorType.Leq => "<=",
                _ => throw new InternalErrorException("Unknown operator type."),
            };
            return $"{this.LExpr} {ch} {this.RExpr}";
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
