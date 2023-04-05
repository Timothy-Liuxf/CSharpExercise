using GoScript.Frontend.Runtime;
using GoScript.Utils;
using System.CodeDom.Compiler;

namespace GoScript.Frontend.AST
{
    public sealed class Compound : Statement
    {
        public const string Indent = "    ";

        public IReadOnlyList<Statement> Statements { get; private init; }

        internal Scope? AttachedScope { get; set; }

        public Compound(IReadOnlyList<Statement> statements, SourceLocation location)
            : base(location)
        {
            this.Statements = statements;
        }

        public override string ToString()
        {
            if (Statements.Count == 0)
            {
                return "{" + Environment.NewLine + "}";
            }
            var result = "";
            foreach (Statement stmt in Statements)
            {
                result += stmt.ToString();
            }
            var endWithNewLine = result.EndsWith(Environment.NewLine);
            var lines = result.Split(Environment.NewLine);
            result = string.Join(Environment.NewLine,
                lines.Take(endWithNewLine ? lines.Length - 1 : lines.Length)
                     .Select((line, _) => Indent + line))
                     + Environment.NewLine;
            return "{" + Environment.NewLine + result + "}";
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
