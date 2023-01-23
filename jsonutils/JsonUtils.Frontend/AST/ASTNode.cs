namespace JsonUtils.Frontend.AST
{
    public abstract class ASTNode
    {
        #region Attributes

        public string[]? StringAttribute { get; set; }

        #endregion

        public abstract void Accept(IVisitor visitor);
    }
}
