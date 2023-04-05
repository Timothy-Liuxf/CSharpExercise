using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    internal class AssignStmt : Statement
    {
        public IList<Expression> AssignedExprs { get; private init; }
        public IList<Expression> Exprs { get; private init; }
        public override bool EndWithNewLine { get; set; } = false;

        public AssignStmt(IList<Expression> assignedExprs, IList<Expression> exprs, SourceLocation location)
            : base(location)
        {
            this.AssignedExprs = assignedExprs;
            this.Exprs = exprs;
        }

        public override string ToString()
        {
            return string.Join(", ", this.AssignedExprs)
                + $" = {string.Join(", ", this.Exprs)}"
                + (EndWithNewLine ? Environment.NewLine : "");
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
