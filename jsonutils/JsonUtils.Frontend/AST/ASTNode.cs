namespace JsonUtils.Frontend.AST
{
    public abstract class ASTNode
    {
        public abstract void Accept(IVisitor visitor);
    }
}
