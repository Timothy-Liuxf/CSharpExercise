using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public sealed class CompoundStmt : Statement
    {
        public IList<Statement> Statements { get; private init; }

        public SourceLocation Location { get; private init; }

        public CompoundStmt(IList<Statement> statements, SourceLocation location)
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
