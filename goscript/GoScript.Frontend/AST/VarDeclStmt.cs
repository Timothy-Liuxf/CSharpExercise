using GoScript.Frontend.Types;
using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public sealed class VarDecl : Statement
    {
        public IReadOnlyList<string> VarNames { get; private init; }
        public GSType? InitType { get; private init; }
        public IReadOnlyList<Expression>? InitExprs { get; private init; }

        public VarDecl(IReadOnlyList<string> varNames, IReadOnlyList<Expression> initExprs, SourceLocation location)
            : base(location)
        {
            this.VarNames = varNames;
            this.InitExprs = initExprs;
        }

        public VarDecl(IReadOnlyList<string> varNames, GSType initType, SourceLocation location)
            : base(location)
        {
            this.VarNames = varNames;
            this.InitType = initType;
        }

        public VarDecl(IReadOnlyList<string> varNames, GSType initType, IReadOnlyList<Expression> initExprs, SourceLocation location)
            : base(location)
        {
            this.VarNames = varNames;
            this.InitType = initType;
            this.InitExprs = initExprs;
        }

        public override string ToString()
        {
            return "var " + string.Join(", ", this.VarNames)
                + (this.InitType is null ? "" : $" {this.InitType}")
                + (this.InitExprs is null ? "" : $" = {string.Join(", ", InitExprs)}");
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }

    public sealed class VarDeclStmt : Statement
    {
        public VarDecl VarDecl { get; private init; }

        public VarDeclStmt(VarDecl varDecl, SourceLocation location)
            : base(location)
        {
            this.VarDecl = varDecl;
        }

        public override string ToString()
        {
            return this.VarDecl.ToString() + Environment.NewLine;
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
