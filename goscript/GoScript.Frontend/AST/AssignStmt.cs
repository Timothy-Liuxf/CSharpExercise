using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    internal class AssignStmt : Statement
    {
        public IReadOnlyList<Expression> AssignedExprs { get; private init; }
        public IReadOnlyList<Expression> Exprs { get; private init; }
        public override bool EndWithNewLine { get; set; } = false;

        public AssignStmt(IEnumerable<Expression> assignedExprs, IEnumerable<Expression> exprs, SourceLocation location)
            : base(location)
        {
            this.AssignedExprs = assignedExprs.ToList();
            this.Exprs = exprs.ToList();
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
