using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public sealed class ContinueStmt : Statement
    {

        public ContinueStmt(SourceLocation location)
            : base(location)
        {
        }

        public override string ToString()
        {
            return $"continue" + Environment.NewLine;
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
