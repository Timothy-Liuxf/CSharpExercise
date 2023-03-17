﻿namespace GoScript.Frontend.AST
{
    internal interface IVisitor
    {
        void Visit(AddExpr addExpr);
        void Visit(IdExpr idExpr);
        void Visit(IntegerRValueExpr integerRValueExpr);
        void Visit(SingleStmt singleStmt);
        void Visit(EmptyStmt emptyStmt);
        void Visit(VarDecl varDecl);
    }
}
