using GoScript.Frontend.AST;
using GoScript.Frontend.Runtime;
using GoScript.Frontend.Types;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace GoScript.Frontend.Translation
{
    internal class Translator : IVisitor
    {
        private ScopeStack scopeStack;
        private readonly IEnumerable<ASTNode> asts;
        private readonly TypeCheck typeCheck;
        private readonly ReleaseHelper releaseHelper;

        public IEnumerable<Statement> Translate()
        {
            foreach (var ast in this.asts)
            {
                try
                {
                    ast.Accept(this.typeCheck);
                    ast.Accept(this);
                }
                catch (CodeErrorException)
                {
                    throw;
                }
                catch (InternalErrorException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new InternalErrorException($"\n=====\n{e.ToString() + e.StackTrace}\n=====\n");
                }
                yield return (ast as Statement) ?? throw new InternalErrorException($"AST: {ast} is not a statement.");
            }
        }

        public Translator(IEnumerable<ASTNode> asts)
        {
            this.asts = asts;
            this.scopeStack = new();
            this.typeCheck = new(scopeStack);
            this.releaseHelper = new();
        }

        private static object ConvertArithmeticConstantValue(ulong constantValue, GSArithmeticType targetType)
        {
            return targetType.IsSigned ? Convert.ChangeType((long)constantValue, targetType.DotNetType)
                : Convert.ChangeType(constantValue, targetType.DotNetType);
        }

        void IVisitor.Visit(VarDecl varDecl)
        {
            var cnt = varDecl.VarNames.Count;
            for (int i = 0; i < cnt; ++i)
            {
                var varName = varDecl.VarNames[i].Item1;
                var rtti = this.scopeStack.LookUpInCurrentScope(varName)
                    ?? throw new InternalErrorException($"At {varDecl.Location}: the symbol of \"{varName}\" has not been built.");
                var type = rtti.Type!;

                if (varDecl.InitExprs == null)   // This means varDecl.InitType must not be null
                {
                    if (type.IsBasic)
                    {
                        rtti.Value = Convert.ChangeType(0, ((GSBasicType)type).DotNetType);
                    }
                    else if (type.IsFunc)
                    {
                        rtti.Value = new FuncValue(null);
                    }
                    else
                    {
                        throw new InternalErrorException(
                            $"At {varDecl.InitType!.Value.Item2}: Unknown type {type}.");
                    }
                }
                else
                {
                    var initExpr = varDecl.InitExprs[i];
                    initExpr.Accept(this);
                    try
                    {
                        AssignValueHelper(rtti, initExpr);
                    }
                    finally
                    {
                        initExpr.Accept(this.releaseHelper);
                    }
                }
            }
        }

        void IVisitor.Visit(VarDeclStmt varDeclStmt)
        {
            var varDecl = varDeclStmt.VarDecl;
            varDecl.Accept(this);
        }

        void IVisitor.Visit(AssignStmt assignStmt)
        {
            var assignedExprs = assignStmt.AssignedExprs;
            var exprs = assignStmt.Exprs;
            var cnt = assignedExprs.Count;
            for (int i = 0; i < cnt; ++i)
            {
                var expr = exprs[i];
                var assignedExpr = (assignedExprs[i] as IdExpr)!;
                expr.Accept(this);
                try
                {
                    assignedExpr.Accept(this);
                    try
                    {
                        var rtti = assignedExpr.RTTI;
                        if (rtti is null)
                        {
                            throw new InternalErrorException($"At {assignedExpr.Location}: the RTTI of \"{assignedExpr.Name}\" has not been built.");
                        }
                        AssignValueHelper(rtti, expr);
                    }
                    finally
                    {
                        assignedExpr.Accept(this.releaseHelper);
                    }
                }
                finally
                {
                    expr.Accept(this.releaseHelper);
                }
            }
        }

        void IVisitor.Visit(DefAssignStmt defAssignStmt)
        {
            var assignedVarNames = defAssignStmt.VarNames;
            var initExprs = defAssignStmt.InitExprs;
            var cnt = assignedVarNames.Count;
            for (int i = 0; i < cnt; ++i)
            {
                var assignedVarName = assignedVarNames[i];
                var initExpr = initExprs[i];
                initExpr.Accept(this);
                try
                {
                    var rtti = this.scopeStack.LookUpInCurrentScope(assignedVarName)
                    ?? throw new InternalErrorException($"At {defAssignStmt.Location}: the symbol of \"{assignedVarName}\" has not been built.");
                    AssignValueHelper(rtti, initExpr);
                }
                finally
                {
                    initExpr.Accept(this.releaseHelper);
                }
            }
        }

        private void AssignValueHelper(RTTI rtti, Expression expr)
        {
            var exprType = expr.Attributes.ExprType!;
            var exprValue = expr.Attributes.Value;
            if (exprType.IsIntegerConstant)
            {
                rtti.Value = ConvertArithmeticConstantValue((ulong)exprValue!, (GSArithmeticType)rtti.Type!);
            }
            else if (exprType.IsBoolConstant)
            {
                rtti.Value = exprValue;
            }
            else
            {
                rtti.Value = exprValue;
            }
        }

        void IVisitor.Visit(IfStmt ifStmt)
        {
            bool done = false;
            foreach (var (cond, branch, _) in ifStmt.ConditionBranches)
            {
                cond.Accept(this);
                try
                {
                    if (cond.Attributes.Value is not bool condValue)
                    {
                        throw new InternalErrorException($"At {cond.Location}: the value of the condition is not a bool.");
                    }
                    if (condValue)
                    {
                        branch.Accept(this);
                        try
                        {
                            ifStmt.Attributes.Value = branch.Attributes.Value;
                        }
                        finally
                        {
                            branch.Accept(this.releaseHelper);
                        }
                        done = true;
                        break;
                    }
                }
                finally
                {
                    cond.Accept(this.releaseHelper);
                }
            }
            if (!done && ifStmt.ElseBranch is not null)
            {
                ifStmt.ElseBranch.Accept(this);
                try
                {
                    ifStmt.Attributes.Value = ifStmt.ElseBranch.Attributes.Value;
                }
                finally
                {
                    ifStmt.ElseBranch.Accept(this.releaseHelper);
                }
            }
        }

        void IVisitor.Visit(ForStmt forStmt)
        {
            this.scopeStack.AttachScope(forStmt.AttachedScope
                ?? throw new InternalErrorException($"At {forStmt.Location}: ForStmt has no attached scope."));
            try
            {
                var initStmt = forStmt.InitStmt;
                var condition = forStmt.Condition;
                var postStmt = forStmt.PostStmt;
                var statements = forStmt.Statements;
                initStmt?.Accept(this);
                initStmt?.Accept(this.releaseHelper);
                if (condition is not null)
                {
                    while (true)
                    {
                        condition.Accept(this);
                        try
                        {
                            if (condition.Attributes.Value is not bool condValue)
                            {
                                throw new InternalErrorException($"At {condition.Location}: the value of the condition is not a bool.");
                            }
                            if (!condValue)
                            {
                                break;
                            }
                        }
                        finally
                        {
                            condition.Accept(this.releaseHelper);
                        }
                        try
                        {
                            statements.Accept(this);
                            statements.Accept(this.releaseHelper);
                        }
                        catch (BreakException)
                        {
                            break;
                        }
                        catch (ContinueException)
                        {
                        }
                        postStmt?.Accept(this);
                        postStmt?.Accept(this.releaseHelper);
                    }
                }
                else
                {
                    while (true)
                    {
                        try
                        {
                            statements.Accept(this);
                            statements.Accept(releaseHelper);
                        }
                        catch (BreakException)
                        {
                            break;
                        }
                        catch (ContinueException)
                        {
                        }
                    }
                }
            }
            finally
            {
                this.scopeStack.DestroyScope();
            }
        }

        void IVisitor.Visit(BreakStmt breakStmt)
        {
            throw new BreakException();
        }

        void IVisitor.Visit(ContinueStmt continueStmt)
        {
            throw new ContinueException();
        }

        void IVisitor.Visit(LogicalOrExpr logicalOrExpr)
        {
            if (logicalOrExpr.IsConstantEvaluated) return;

            var lExpr = logicalOrExpr.LExpr;
            var rExpr = logicalOrExpr.RExpr;

            lExpr.Accept(this);
            if ((bool)lExpr.Attributes.Value!)
            {
                logicalOrExpr.Attributes.Value = true;
            }
            else
            {
                rExpr.Accept(this);
                logicalOrExpr.Attributes.Value = rExpr.Attributes.Value;
            }
        }

        void IVisitor.Visit(LogicalAndExpr logicalAndExpr)
        {
            if (logicalAndExpr.IsConstantEvaluated) return;

            var lExpr = logicalAndExpr.LExpr;
            var rExpr = logicalAndExpr.RExpr;

            lExpr.Accept(this);
            if (!(bool)lExpr.Attributes.Value!)
            {
                logicalAndExpr.Attributes.Value = false;
            }
            else
            {
                rExpr.Accept(this);
                logicalAndExpr.Attributes.Value = rExpr.Attributes.Value;
            }
        }

        void IVisitor.Visit(ComparisonExpr comparisonExpr)
        {
            if (comparisonExpr.IsConstantEvaluated) return;

            var op = comparisonExpr.Operator switch
            {
                ComparisonExpr.OperatorType.Equ => "==",
                ComparisonExpr.OperatorType.Neq => "!=",
                ComparisonExpr.OperatorType.Gre => ">",
                ComparisonExpr.OperatorType.Les => "<",
                ComparisonExpr.OperatorType.Geq => ">=",
                ComparisonExpr.OperatorType.Leq => "<=",
                _ => throw new InternalErrorException($"At {comparisonExpr.Location}: Invalid operator type."),
            };

            var lExpr = comparisonExpr.LExpr;
            var rExpr = comparisonExpr.RExpr;
            lExpr.Accept(this);
            rExpr.Accept(this);
            var (lOp, rOp) = EvaluateArithmeticExpression(comparisonExpr);

            var res = (object)(op switch
            {
                "==" => lOp == rOp,
                "!=" => lOp != rOp,
                ">" => lOp > rOp,
                "<" => lOp < rOp,
                ">=" => lOp >= rOp,
                _ => lOp <= rOp,
            });
            comparisonExpr.Attributes.Value = res;
        }

        void IVisitor.Visit(AdditiveExpr additiveExpr)
        {
            if (additiveExpr.IsConstantEvaluated) return;

            char op = additiveExpr.Operator switch
            {
                AdditiveExpr.OperatorType.Add => '+',
                AdditiveExpr.OperatorType.Sub => '-',
                _ => throw new InternalErrorException($"At {additiveExpr.Location}: Invalid operator type."),
            };

            var lExpr = additiveExpr.LExpr;
            var rExpr = additiveExpr.RExpr;
            lExpr.Accept(this);
            rExpr.Accept(this);
            var (lOp, rOp) = EvaluateArithmeticExpression(additiveExpr);
            additiveExpr.Attributes.Value = (object)(
                    op == '+' ? lOp + rOp : lOp - rOp
                ) ?? throw new InternalErrorException($"Invalid \'{op}\' at {additiveExpr.Location}.");
        }

        void IVisitor.Visit(MultiplicativeExpr multiplicativeExpr)
        {
            if (multiplicativeExpr.IsConstantEvaluated) return;

            char op = multiplicativeExpr.Operator switch
            {
                MultiplicativeExpr.OperatorType.Mul => '*',
                MultiplicativeExpr.OperatorType.Div => '/',
                MultiplicativeExpr.OperatorType.Mod => '%',
                _ => throw new InternalErrorException($"At {multiplicativeExpr.Location}: Invalid operator type."),
            };

            var lExpr = multiplicativeExpr.LExpr;
            var rExpr = multiplicativeExpr.RExpr;
            lExpr.Accept(this);
            rExpr.Accept(this);
            var (lOp, rOp) = EvaluateArithmeticExpression(multiplicativeExpr);

            var res = (object)(op switch
            {
                '*' => lOp * rOp,
                '/' => lOp / rOp,
                _ => lOp % rOp,
            });
            if (res.GetType() != ((GSBasicType)multiplicativeExpr.Attributes.ExprType!).DotNetType)
            {
                res = Convert.ChangeType(res, ((GSBasicType)multiplicativeExpr.Attributes.ExprType!).DotNetType);
            }
            multiplicativeExpr.Attributes.Value = res;
        }

        private (dynamic lOp, dynamic rOp) EvaluateArithmeticExpression(ArithmeticExpression arithmeticExpression)
        {
            var lExpr = arithmeticExpression.LExpr;
            var rExpr = arithmeticExpression.RExpr;
            var lType = lExpr.Attributes.ExprType!;
            var rType = rExpr.Attributes.ExprType!;
            var lOp = (dynamic)lExpr.Attributes.Value!;
            var rOp = (dynamic)rExpr.Attributes.Value!;
            if (lType.IsArithmetic && rType.IsIntegerConstant)
            {
                rOp = (dynamic)ConvertArithmeticConstantValue((ulong)rOp, (GSArithmeticType)lType);
            }
            else if (rType.IsArithmetic && lType.IsIntegerConstant)
            {
                lOp = (dynamic)ConvertArithmeticConstantValue((ulong)lOp, (GSArithmeticType)rType);
            }
            return (lOp, rOp);
        }

        void IVisitor.Visit(UnaryExpr unaryExpr)
        {
            if (unaryExpr.IsConstantEvaluated) return;

            var operand = unaryExpr.Operand;
            operand.Accept(this);
            var exprType = unaryExpr.Attributes.ExprType!;
            var operandValue = operand.Attributes.Value!;

            switch (unaryExpr.Operator)
            {
                case UnaryExpr.OperatorType.Neg:
                    if (exprType.IsArithmetic)
                    {
                        unaryExpr.Attributes.Value = -(dynamic)operandValue;
                    }
                    else if (exprType.IsIntegerConstant)
                    {
                        unaryExpr.Attributes.Value = (ulong)(-(long)(ulong)operandValue);
                    }
                    break;
                case UnaryExpr.OperatorType.Not:
                    unaryExpr.Attributes.Value = !(bool)operandValue;
                    break;
            }
        }

        void IVisitor.Visit(EmptyStmt emptyStmt)
        {
        }

        void IVisitor.Visit(SingleStmt singleStmt)
        {
            singleStmt.Expr.Accept(this);
            try
            {
                if (singleStmt.Echo)
                {
                    singleStmt.Attributes.Value = singleStmt.Expr.Attributes.Value;
                }
            }
            finally
            {
                singleStmt.Expr.Accept(this.releaseHelper);
            }
        }

        void IVisitor.Visit(IdExpr idExpr)
        {
            var rtti = idExpr.RTTI;
            if (rtti is null)
            {
                throw new InternalErrorException($"At {idExpr.Location}: Symbol \"{idExpr.Name}\" has not been built.");
            }
            idExpr.Attributes.Value = rtti.Value;
        }

        void IVisitor.Visit(IntegerConstantExpr integerConstantExpr)
        {
        }

        void IVisitor.Visit(BoolConstantExpr boolConstantExpr)
        {
        }

        void IVisitor.Visit(Compound compound)
        {
            this.scopeStack.AttachScope(compound.AttachedScope
                ?? throw new InternalErrorException($"At {compound.Location}: CompoundStmt has no attached scope."));
            try
            {
                var statements = compound.Statements;
                object? lastValue = null;
                foreach (var statement in statements)
                {
                    statement.Accept(this);
                    lastValue = statement.Attributes.Value;
                    statement.Accept(this.releaseHelper);
                }
                compound.Attributes.Value = lastValue;
            }
            finally
            {
                this.scopeStack.DestroyScope();
            }
        }

        void IVisitor.Visit(CompoundStmt compoundStmt)
        {
            var compound = compoundStmt.Compound;
            compound.Accept(this);
            try
            {
                compoundStmt.Attributes.Value = compound.Attributes.Value;
            }
            finally
            {
                compound.Accept(this.releaseHelper);
            }
        }

        void IVisitor.Visit(FuncExpr funcExpr)
        {
        }
    }
}
