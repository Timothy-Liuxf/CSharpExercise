using GoScript.Frontend;
using GoScript.Frontend.Types;

namespace GoScript.Frontend.AST
{
    public sealed class VarDecl : Statement
    {
        public override GSType? StmtType
        {
            get => new GSNilType();
        }
        public string VarName { get; private init; }
        public string? InitType { get; private init; }
        public Expression? InitExpr { get; private init; }

        public VarDecl(string varName, Expression initExpr)
        {
            VarName = varName;
            InitExpr = initExpr;
        }

        public VarDecl(string varName, string initType)
        {
            VarName = varName;
            InitType = initType;
        }

        public VarDecl(string varName, string initType, Expression initExpr)
        {
            VarName = varName;
            InitType = initType;
            InitExpr = initExpr;
        }

        public override string ToString()
        {
            return "var " + this.VarName
                + (this.InitType == null ? "" : $" {this.InitType}")
                + (this.InitExpr == null ? "" : $" = {this.InitExpr}")
                + Environment.NewLine;
        }
    }
}
