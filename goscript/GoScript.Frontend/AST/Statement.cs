using GoScript.Frontend.Types;

namespace GoScript.Frontend.AST
{
    public abstract class Statement : ASTNode
    {
        public struct AttributesList
        {
            public GSType? StmtType { get; internal set; }
            public object? Value { get; internal set; }
        }

        public AttributesList Attributes = new();
    }

    public sealed class SingleStmt : Statement
    {
        public Expression Expr { get; private init; }

        public bool Echo { get; private init; }

        public SingleStmt(Expression expr, bool echo)
        {
            Expr = expr;
            this.Echo = echo;
        }

        public override string ToString()
        {
            return this.Expr.ToString() + (this.Echo ? "" : ";") + Environment.NewLine;
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }

    public sealed class EmptyStmt : Statement
    {
        public override string ToString()
        {
            return Environment.NewLine;
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
