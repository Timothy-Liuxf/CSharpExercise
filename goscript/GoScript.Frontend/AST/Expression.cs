using GoScript.Frontend.Types;

namespace GoScript.Frontend.AST
{
    public abstract class Expression : ASTNode
    {
        public struct AttributesList
        {
            public GSType? ExprType { get; internal set; }
            public object? Value { get; internal set; }
        }

        public AttributesList Attributes = new();
    }
}
