using GoScript.Frontend.Runtime;
using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public sealed class CompoundStmt : Statement
    {
        public IReadOnlyList<Statement> Statements { get; private init; }

        public SourceLocation Location { get; private init; }

        internal Scope? AttachedScope { get; set; }

        public CompoundStmt(IReadOnlyList<Statement> statements, SourceLocation location)
        {
            this.Statements = statements;
            this.Location = location;
        }

        public override string ToString()
        {
            string result = "{" + Environment.NewLine;
            foreach (Statement stmt in Statements)
            {
                result += stmt.ToString();
            }
            result += "}" + Environment.NewLine;
            return result;
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
