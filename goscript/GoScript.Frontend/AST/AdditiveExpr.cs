using GoScript.Frontend.Types;
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
        public SourceLocation Location { get; private init; }

        public AdditiveExpr(Expression lExpr, Expression rExpr, OperatorType @operator, SourceLocation location)
        {
            this.LExpr = lExpr;
            this.RExpr = rExpr;
            this.Operator = @operator;
            this.Location = location;
        }

        public override string ToString()
        {
            return $"{this.LExpr} + {this.RExpr}";
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
