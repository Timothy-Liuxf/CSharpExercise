namespace JsonUtils.Frontend.AST
{
    public interface IVisitor
    {
        void Visit(ArrayObject arrayObj);
        void Visit(ClassObject classObj);
        void Visit(BooleanValue booleanValue);
        void Visit(NumberValue numberValue);
        void Visit(StringValue stringValue);
        void Visit(NullValue nullValue);
    }
}
