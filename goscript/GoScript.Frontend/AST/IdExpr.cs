using GoScript.Frontend.Types;
using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public sealed class IdExpr : Expression
    {
        public string Name { get; private init; }
        public SourceLocation Location { get; private init; }

        public IdExpr(string name, SourceLocation location)
        {
            this.Name = name;
            this.Location = location;
        }

        public override string ToString()
        {
            return Name;
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
