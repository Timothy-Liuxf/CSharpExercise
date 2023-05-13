using GoScript.Frontend.AST;

namespace GoScript.Frontend.Translation
{
    internal class ReleaseHelper : IVisitor
    {
        void IVisitor.Visit(VarDecl varDecl)
        {
            varDecl.Attributes.Value = null;
            if (varDecl.InitExprs is not null)
            {
                foreach (var initExpr in varDecl.InitExprs.Reverse())
                {
                    initExpr.Accept(this);
                }
            }
        }

        void IVisitor.Visit(VarDeclStmt varDeclStmt)
        {
            varDeclStmt.Attributes.Value = null;
            varDeclStmt.VarDecl.Accept(this);
        }

        void IVisitor.Visit(AssignStmt assignStmt)
        {
            assignStmt.Attributes.Value = null;
            var assignedExprs = assignStmt.AssignedExprs;
            var exprs = assignStmt.Exprs;
            var cnt = assignedExprs.Count;
            for (int i = cnt - 1; i >= 0; --i)
            {
                var assignedExpr = assignedExprs[i];
                var expr = exprs[i];
                assignedExpr.Accept(this);
                expr.Accept(this);
            }
        }

        void IVisitor.Visit(DefAssignStmt defAssignStmt)
        {
            defAssignStmt.Attributes.Value = null;
            foreach (var initExpr in defAssignStmt.InitExprs.Reverse())
            {
                initExpr.Accept(this);
            }
        }

        void IVisitor.Visit(IfStmt ifStmt)
        {
        }

        void IVisitor.Visit(ForStmt forStmt)
        {
        }

        void IVisitor.Visit(BreakStmt breakStmt)
        {
        }

        void IVisitor.Visit(ContinueStmt continueStmt)
        {
        }

        void IVisitor.Visit(LogicalOrExpr logicalOrExpr)
        {
            if (logicalOrExpr.IsConstantEvaluated) return;

            logicalOrExpr.Attributes.Value = null;
            if (logicalOrExpr.RExpr.Attributes.Value is not null)
            {
                logicalOrExpr.RExpr.Accept(this);
            }
            logicalOrExpr.LExpr.Accept(this);
        }

        void IVisitor.Visit(LogicalAndExpr logicalAndExpr)
        {
            if (logicalAndExpr.IsConstantEvaluated) return;

            logicalAndExpr.Attributes.Value = null;
            if (logicalAndExpr.RExpr.Attributes.Value is not null)
            {
                logicalAndExpr.RExpr.Accept(this);
            }
            logicalAndExpr.LExpr.Accept(this);
        }

        void IVisitor.Visit(ComparisonExpr comparisonExpr)
        {
            if (comparisonExpr.IsConstantEvaluated) return;

            comparisonExpr.Attributes.Value = null;
            comparisonExpr.RExpr.Accept(this);
            comparisonExpr.LExpr.Accept(this);
        }

        void IVisitor.Visit(AdditiveExpr additiveExpr)
        {
            if (additiveExpr.IsConstantEvaluated) return;

            additiveExpr.Attributes.Value = null;
            additiveExpr.RExpr.Accept(this);
            additiveExpr.LExpr.Accept(this);
        }

        void IVisitor.Visit(MultiplicativeExpr multiplicativeExpr)
        {
            if (multiplicativeExpr.IsConstantEvaluated) return;

            multiplicativeExpr.Attributes.Value = null;
            multiplicativeExpr.RExpr.Accept(this);
            multiplicativeExpr.LExpr.Accept(this);
        }

        void IVisitor.Visit(UnaryExpr unaryExpr)
        {
            if (unaryExpr.IsConstantEvaluated) return;

            unaryExpr.Attributes.Value = null;
            unaryExpr.Operand.Accept(this);
        }

        void IVisitor.Visit(EmptyStmt emptyStmt)
        {
        }

        void IVisitor.Visit(SingleStmt singleStmt)
        {
            singleStmt.Attributes.Value = null;
        }

        void IVisitor.Visit(IdExpr idExpr)
        {
            idExpr.Attributes.Value = null;
        }

        void IVisitor.Visit(IntegerConstantExpr integerConstantExpr)
        {
        }

        void IVisitor.Visit(BoolConstantExpr boolConstantExpr)
        {
        }

        void IVisitor.Visit(Compound compound)
        {
            compound.Attributes.Value = null;
        }

        void IVisitor.Visit(CompoundStmt compoundStmt)
        {
            compoundStmt.Attributes.Value = null;
            compoundStmt.Compound.Accept(this);

        }

        void IVisitor.Visit(FuncExpr funcExpr)
        {
            throw new NotImplementedException(nameof(FuncExpr));
        }
    }
}
