using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public sealed class BreakStmt : Statement
    {

        public BreakStmt(SourceLocation location)
            : base(location)
        {
        }

        public override string ToString()
        {
            return $"break" + Environment.NewLine;
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
