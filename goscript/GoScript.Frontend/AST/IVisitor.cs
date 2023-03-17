namespace GoScript.Frontend.AST
{
    internal interface IVisitor
    {
        void Visit(AdditiveExpr additiveExpr);
        void Visit(MultiplicativeExpr multiplicativeExpr);
        void Visit(UnaryExpr unaryExpr);
        void Visit(IdExpr idExpr);
        void Visit(IntegerRValueExpr integerRValueExpr);
        void Visit(SingleStmt singleStmt);
        void Visit(EmptyStmt emptyStmt);
        void Visit(VarDecl varDecl);
    }
}
