using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public abstract class ASTNode
    {
        public SourceLocation Location { get; private init; }

        public ASTNode(SourceLocation location)
        {
            this.Location = location;
        }

        internal abstract void Accept(IVisitor visitor);
    }
}
