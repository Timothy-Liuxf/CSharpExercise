using GoScript.Frontend.Runtime;
using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public sealed class Compound
    {
        public IReadOnlyList<Statement> Statements { get; private init; }

        public SourceLocation Location { get; private init; }

        internal Scope? AttachedScope { get; set; }

        public Compound(IReadOnlyList<Statement> statements, SourceLocation location)
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
            result += "}";
            return result;
        }
    }

    public sealed class CompoundStmt : Statement
    {
        public Compound Compound { get; private init; }

        public CompoundStmt(Compound compound)
        {
            this.Compound = compound;
        }

        public override string ToString()
        {
            return this.Compound.ToString() + Environment.NewLine;
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
