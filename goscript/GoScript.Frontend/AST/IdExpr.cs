using GoScript.Frontend.Runtime;
using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public sealed class IdExpr : Expression
    {
        internal RTTI? RTTI { get; set; }

        public string Name { get; private init; }

        public IdExpr(string name, SourceLocation location) : base(location)
        {
            this.Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool IsIdExpr => true;

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
