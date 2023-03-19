using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    internal class AssignStmt : Statement
    {
        public IList<Expression> AssignedExprs { get; private init; }
        public IList<Expression> Exprs { get; private init; }
        public SourceLocation Location { get; private init; }

        public AssignStmt(IList<Expression> assignedExprs, IList<Expression> exprs, SourceLocation location)
        {
            this.AssignedExprs = assignedExprs;
            this.Exprs = exprs;
            this.Location = location;
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
