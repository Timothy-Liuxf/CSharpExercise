using GoScript.Frontend;
using GoScript.Frontend.Types;
using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public sealed class VarDecl : Statement
    {
        public string VarName { get; private init; }
        public string? InitType { get; private init; }
        public Expression? InitExpr { get; private init; }
        public SourceLocation Location { get; private init; }

        public VarDecl(string varName, Expression initExpr, SourceLocation location)
        {
            VarName = varName;
            InitExpr = initExpr;
            Location = location;
        }

        public VarDecl(string varName, string initType, SourceLocation location)
        {
            VarName = varName;
            InitType = initType;
            Location = location;
        }

        public VarDecl(string varName, string initType, Expression initExpr, SourceLocation location)
        {
            VarName = varName;
            InitType = initType;
            InitExpr = initExpr;
            Location = location;
        }

        public override string ToString()
        {
            return "var " + this.VarName
                + (this.InitType == null ? "" : $" {this.InitType}")
                + (this.InitExpr == null ? "" : $" = {this.InitExpr}")
                + Environment.NewLine;
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
