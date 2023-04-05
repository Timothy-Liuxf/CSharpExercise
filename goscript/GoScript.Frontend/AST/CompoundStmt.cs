using GoScript.Frontend.Runtime;
using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public sealed class Compound : Statement
    {
        public IReadOnlyList<Statement> Statements { get; private init; }

        internal Scope? AttachedScope { get; set; }

        public Compound(IReadOnlyList<Statement> statements, SourceLocation location)
            : base(location)
        {
            this.Statements = statements;
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

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }

    public sealed class CompoundStmt : Statement
    {
        public Compound Compound { get; private init; }

        public CompoundStmt(Compound compound, SourceLocation location)
            : base(location)
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
