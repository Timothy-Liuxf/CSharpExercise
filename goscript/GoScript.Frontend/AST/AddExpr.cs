using GoScript.Frontend.Types;

namespace GoScript.Frontend.AST
{
    public sealed class AddExpr : Expression
    {
        public Expression LExpr { get; private init; }
        public Expression RExpr { get; private init; }

        public override object? Value { get; internal set; }
        public override GSType? ExprType { get; internal set; }

        public AddExpr(Expression lExpr, Expression rExpr)
        {
            this.LExpr = lExpr;
            this.RExpr = rExpr;
        }

        public override string ToString()
        {
            return $"{this.LExpr} + {this.RExpr}";
        }
    }
}
