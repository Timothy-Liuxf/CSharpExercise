using GoScript.Frontend.Types;

namespace GoScript.Frontend.AST
{
    public abstract class Statement : ASTNode
    {
        public abstract GSType? StmtType { get; }
    }

    public sealed class SingleStmt : Statement
    {
        public Expression Expr { get; private init; }

        public override GSType? StmtType => nilType ? new GSNilType() : Expr.ExprType;

        private readonly bool nilType;

        public SingleStmt(Expression expr, bool nilType)
        {
            Expr = expr;
            this.nilType = nilType;
        }

        public override string ToString()
        {
            return this.Expr.ToString() + (this.nilType ? "" : ";") + Environment.NewLine;
        }
    }

    public class EmptyStmt : Statement
    {
        public override GSType? StmtType => new GSNilType();

        public override string ToString()
        {
            return Environment.NewLine;
        }
    }
}
