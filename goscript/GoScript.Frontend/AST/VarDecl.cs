using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public sealed class VarDecl : Statement
    {
        public IReadOnlyList<string> VarNames { get; private init; }
        public string? InitType { get; private init; }
        public IReadOnlyList<Expression>? InitExprs { get; private init; }
        public SourceLocation Location { get; private init; }

        public VarDecl(IReadOnlyList<string> varNames, IReadOnlyList<Expression> initExprs, SourceLocation location)
        {
            VarNames = varNames;
            InitExprs = initExprs;
            Location = location;
        }

        public VarDecl(IReadOnlyList<string> varNames, string initType, SourceLocation location)
        {
            VarNames = varNames;
            InitType = initType;
            Location = location;
        }

        public VarDecl(IReadOnlyList<string> varNames, string initType, IReadOnlyList<Expression> initExprs, SourceLocation location)
        {
            VarNames = varNames;
            InitType = initType;
            InitExprs = initExprs;
            Location = location;
        }

        public override string ToString()
        {
            return "var " + string.Join(", ", this.VarNames)
                + (this.InitType is null ? "" : $" {this.InitType}")
                + (this.InitExprs is null ? "" : $" = {string.Join(", ", InitExprs)}")
                + Environment.NewLine;
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
