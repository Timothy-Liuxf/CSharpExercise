using GoScript.Frontend.Types;
using GoScript.Utils;
using System.Text;

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

        public Statement(SourceLocation location) : base(location)
        {
        }

        public virtual bool EndWithNewLine
        {
            get => true;
            set
            {
                // throw new InternalErrorException(
                //     $"{this.GetType().Name} always ends with newline");
            }
        }

    }

    public sealed class SingleStmt : Statement
    {
        public Expression Expr { get; private init; }

        public bool Echo { get; private init; }

        public override bool EndWithNewLine { get; set; } = false;

        public SingleStmt(Expression expr, bool echo, SourceLocation location)
            : base(location)
        {
            Expr = expr;
            this.Echo = echo;
        }

        public override string ToString()
        {
            return this.Expr.ToString() + (this.Echo ? "" : ";")
                + (this.EndWithNewLine ? Environment.NewLine : "");
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }

    public sealed class EmptyStmt : Statement
    {
        public override string ToString()
        {
            return Environment.NewLine;
        }

        public EmptyStmt(SourceLocation location)
            : base(location)
        {
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
