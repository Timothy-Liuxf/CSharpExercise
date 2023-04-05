namespace GoScript.Frontend.AST
{
    internal interface IVisitor
    {
        void Visit(LogicalOrExpr logicalOrExpr);
        void Visit(LogicalAndExpr logicalAndExpr);
        void Visit(ComparisonExpr comparisonExpr);
        void Visit(AdditiveExpr additiveExpr);
        void Visit(MultiplicativeExpr multiplicativeExpr);
        void Visit(UnaryExpr unaryExpr);
        void Visit(IdExpr idExpr);
        void Visit(IntegerConstantExpr integerLiteralExpr);
        void Visit(BoolConstantExpr boolLiteralExpr);
        void Visit(SingleStmt singleStmt);
        void Visit(EmptyStmt emptyStmt);
        void Visit(VarDecl varDecl);
        void Visit(VarDeclStmt varDeclStmt);
        void Visit(IfStmt ifStmt);
        void Visit(ForStmt forStmt);
        void Visit(Compound compound);
        void Visit(CompoundStmt compoundStmt);
        void Visit(AssignStmt assignStmt);
        void Visit(DefAssignStmt defAssignStmt);
    }
}
