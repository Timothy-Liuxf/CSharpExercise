using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public sealed class LogicalOrExpr : Expression
    {
        public Expression LExpr { get; private init; }
        public Expression RExpr { get; private init; }
        public SourceLocation Location { get; private init; }

        public LogicalOrExpr(Expression lExpr, Expression rExpr, SourceLocation location)
        {
            this.LExpr = lExpr;
            this.RExpr = rExpr;
            this.Location = location;
        }

        public override string ToString()
        {
            return $"{this.LExpr} || {this.RExpr}";
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
