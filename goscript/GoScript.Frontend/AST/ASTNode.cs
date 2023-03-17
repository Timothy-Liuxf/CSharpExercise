using System.Data;

namespace GoScript.Frontend.AST
{
    public abstract class ASTNode
    {
        internal abstract void Accept(IVisitor visitor);
    }
}
