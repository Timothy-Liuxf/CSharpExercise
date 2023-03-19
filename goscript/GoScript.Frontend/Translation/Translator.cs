using GoScript.Frontend.AST;
using GoScript.Frontend.Runtime;
using GoScript.Frontend.Types;
using System.Reflection;

namespace GoScript.Frontend.Translation
{
    internal class Translator : IVisitor
    {
        private ScopeStack scopeStack;
        private readonly IEnumerable<ASTNode> asts;
        private readonly TypeCheck typeCheck;

        public IEnumerable<Statement> Translate()
        {
            foreach (var ast in this.asts)
            {
                try
                {
                    ast.Accept(this.typeCheck);
                    ast.Accept(this);
                }
                catch
                {
                    throw;
                }
                yield return (ast as Statement) ?? throw new InternalErrorException($"AST: {ast} is not a statement.");
            }
        }

        public Translator(IEnumerable<ASTNode> asts)
        {
            this.asts = asts;
            this.scopeStack = new();
            this.typeCheck = new(scopeStack);
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
                var varName = varDecl.VarNames[i];
                var rtti = this.scopeStack.LookUp(varName)
                    ?? throw new InternalErrorException($"At {varDecl.Location}: the symbol of \"{varName}\" has not been built.");
                var type = rtti.Type!;

                if (varDecl.InitExprs == null)   // This means varDecl.InitType must not be null
                {
                    rtti.Value = Convert.ChangeType(0, ((GSBasicType)type).DotNetType);
                }
                else
                {
                    var initExpr = varDecl.InitExprs[i];
                    initExpr.Accept(this);
                    var exprType = initExpr.Attributes.ExprType!;
                    if (varDecl.InitType == null)
                    {
                        if (exprType.IsIntegerConstant)
                        {
                            rtti.Value = ConvertArithmeticConstantValue((ulong)initExpr.Attributes.Value!, (GSInt64)type);
                        }
                        else if (exprType.IsBoolConstant)
                        {
                            rtti.Value = initExpr.Attributes.Value;
                        }
                        else
                        {
                            rtti.Value = initExpr.Attributes.Value;
                        }
                    }
                    else
                    {
                        if (exprType.IsIntegerConstant)
                        {
                            rtti.Value = ConvertArithmeticConstantValue((ulong)initExpr.Attributes.Value!, (GSArithmeticType)type);
                        }
                        else if (exprType.IsBoolConstant)
                        {
                            rtti.Value = initExpr.Attributes.Value;
                        }
                        else
                        {
                            rtti.Value = initExpr.Attributes.Value;
                        }
                    }
                }
            }
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
            if (singleStmt.Echo)
            {
                singleStmt.Attributes.Value = singleStmt.Expr.Attributes.Value;
            }
        }

        void IVisitor.Visit(IdExpr idExpr)
        {
            var rtti = this.scopeStack.LookUp(idExpr.Name) ?? throw new InternalErrorException($"At {idExpr.Location}: Symbol \"{idExpr.Name}\" has not been built.");
            idExpr.Attributes.Value = rtti.Value;
        }

        void IVisitor.Visit(IntegerConstantExpr integerConstantExpr)
        {
        }

        void IVisitor.Visit(BoolConstantExpr boolConstantExpr)
        {
        }

        void IVisitor.Visit(CompoundStmt compoundStmt)
        {
            this.scopeStack.AttachScope(compoundStmt.AttachedScope
                ?? throw new InternalErrorException($"At {compoundStmt.Location}: CompoundStmt has no attached scope."));
            var statements = compoundStmt.Statements;
            foreach (var statement in statements)
            {
                statement.Accept(this);
            }
            if (statements.Count > 0)
            {
                var lastStmt = statements.Last();
                compoundStmt.Attributes.Value = lastStmt.Attributes.Value;
            }
            this.scopeStack.CloseScope();
        }
    }
}
