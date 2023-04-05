using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public sealed class DefAssignStmt : Statement
    {
        public IReadOnlyList<string> VarNames { get; private init; }
        public IReadOnlyList<Expression> InitExprs { get; private init; }
        public override bool EndWithNewLine { get; set; } = false;

        public DefAssignStmt(IReadOnlyList<string> varNames, IReadOnlyList<Expression> initExprs, SourceLocation location)
            : base(location)
        {
            VarNames = varNames;
            InitExprs = initExprs;
        }

        public override string ToString()
        {
            return string.Join(", ", this.VarNames)
                + $" := {string.Join(", ", this.InitExprs)}"
                + (EndWithNewLine ? Environment.NewLine : "");
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
