using GoScript.Frontend.Types;
using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public abstract class Expression : ASTNode
    {
        public Expression(SourceLocation location)
            : base(location)
        {
        }

        public struct AttributesList
        {
            public GSType? ExprType { get; internal set; }
            public object? Value { get; internal set; }
        }

        public AttributesList Attributes = new();

        public bool IsConstantEvaluated { get; set; } = false;

        public virtual bool IsIdExpr { get; } = false;
    }
}
