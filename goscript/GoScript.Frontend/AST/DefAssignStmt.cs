using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public sealed class DefAssignStmt : Statement
    {
        public IReadOnlyList<string> VarNames { get; private init; }
        public IReadOnlyList<Expression> InitExprs { get; private init; }
        public SourceLocation Location { get; private init; }

        public DefAssignStmt(IReadOnlyList<string> varNames, IReadOnlyList<Expression> initExprs, SourceLocation location)
        {
            VarNames = varNames;
            InitExprs = initExprs;
            Location = location;
        }

        public override string ToString()
        {
            return string.Join(", ", this.VarNames)
                + $" := {string.Join(", ", InitExprs)}"
                + Environment.NewLine;
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
