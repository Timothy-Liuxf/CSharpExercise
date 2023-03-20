using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public abstract class ArithmeticExpression : Expression
    {
        public Expression LExpr { get; private init; }
        public Expression RExpr { get; private init; }

        public ArithmeticExpression(Expression lExpr, Expression rExpr, SourceLocation location)
            : base(location)
        {
            this.LExpr = lExpr;
            this.RExpr = rExpr;
        }
    }
}
