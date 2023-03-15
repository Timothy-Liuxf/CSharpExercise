using GoScript.Frontend.Types;

namespace GoScript.Frontend.AST
{
    public abstract class Expression : ASTNode
    {
        public abstract object? Value { get; internal set; }

        public abstract GSType? ExprType { get; internal set; }
    }
}
