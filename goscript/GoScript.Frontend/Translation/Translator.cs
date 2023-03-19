using GoScript.Frontend.AST;
using GoScript.Frontend.Runtime;
using GoScript.Frontend.Types;
using System.Reflection;

namespace GoScript.Frontend.Translation
{
    internal class Translator : IVisitor
    {
        private ScopeStack scopeStack = new();
        private readonly IEnumerable<ASTNode> asts;

        public IEnumerable<Statement> Translate()
        {
            foreach (var ast in this.asts)
            {
                try
                {
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
        }

        private static object ConvertArithmeticLiteralValue(ulong literalValue, GSArithmeticType targetType)
        {
            return targetType.IsSigned ? Convert.ChangeType((long)literalValue, targetType.DotNetType)
                : Convert.ChangeType(literalValue, targetType.DotNetType);
        }

        void IVisitor.Visit(VarDecl varDecl)
        {
            if (varDecl.InitExprs == null && varDecl.InitType == null)
            {
                throw new InternalErrorException($"At {varDecl.Location}: InitExprs and InitType shouldn't be null at the same time.");
            }

            if (varDecl.InitExprs is not null && varDecl.VarNames.Count != varDecl.InitExprs.Count)
            {
                throw new SyntaxErrorException($"At {varDecl.Location}: The number of variables doesn't match the number of initializers.");
            }

            foreach (var varname in varDecl.VarNames)
            {
                if (this.scopeStack.ContainsInCurrentScope(varname))
                {
                    throw new ConflictException($"Conflict at {varDecl.Location}: the name \"{varname}\" has already defined.");
                }
            }

            var cnt = varDecl.VarNames.Count;
            var rttis = new List<RTTI>();
            for (int i = 0; i < cnt; ++i)
            {
                var rtti = new RTTI();
                if (varDecl.InitType != null)
                {
                    var gsType = GSBasicType.ParseBasicType(varDecl.InitType);
                    if (gsType is not null)
                    {
                        rtti.Type = gsType;
                    }
                    else
                    {
                        throw new SymbolNotFoundException($"At {varDecl.Location}: \"{varDecl.InitType}\" is not a valid type.");
                    }
                }
                rttis.Add(rtti);

                varDecl.Attributes.StmtType = null;
                if (varDecl.InitExprs == null)   // This means varDecl.InitType must not be null
                {
                    rtti.Value = Convert.ChangeType(0, ((GSBasicType)rtti.Type!).DotNetType);
                }
                else
                {
                    var initExpr = varDecl.InitExprs[i];
                    var varName = varDecl.VarNames[i];
                    initExpr.Accept(this);
                    var exprType = initExpr.Attributes.ExprType!;
                    if (varDecl.InitType == null)
                    {
                        if (exprType.IsIntegerLiteral)
                        {
                            var type = GSInt64.Instance;
                            rtti.Type = type;
                            rtti.Value = ConvertArithmeticLiteralValue((ulong)initExpr.Attributes.Value!, type);
                        }
                        else if (exprType.IsBoolLiteral)
                        {
                            rtti.Type = GSBool.Instance;
                            rtti.Value = initExpr.Attributes.Value;
                        }
                        else
                        {
                            rtti.Type = exprType;
                            rtti.Value = initExpr.Attributes.Value;
                        }
                    }
                    else
                    {
                        if (exprType.IsIntegerLiteral)
                        {
                            if (rtti.Type!.IsArithmetic)
                            {
                                rtti.Value = ConvertArithmeticLiteralValue((ulong)initExpr.Attributes.Value!, (GSArithmeticType)rtti.Type);
                            }
                            else
                            {
                                throw new InternalErrorException($"Unknown type {rtti.Type} at {varDecl.Location}.");
                            }
                        }
                        else if (exprType.IsBoolLiteral)
                        {
                            if (rtti.Type!.IsBool)
                            {
                                rtti.Value = initExpr.Attributes.Value;
                            }
                            else
                            {
                                throw new InvalidOperationException(
                                    $"At {varDecl.Location}: cannot use bool constant {initExpr} to init {rtti.Type} variable \'{varName}\'.");
                            }
                        }
                        else
                        {
                            if (rtti.Type! != exprType)
                            {
                                throw new InvalidOperationException($"Mismatched type {rtti.Type} and {exprType} at {varDecl.Location}.");
                            }
                            rtti.Value = initExpr.Attributes.Value;
                        }
                    }
                }
            }

            for (int i = 0; i < cnt; ++i)
            {
                this.scopeStack.Add(varDecl.VarNames[i], rttis[i]);
            }
        }

        void IVisitor.Visit(AdditiveExpr additiveExpr)
        {
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
            if (lType.IsArithmetic && rType.IsArithmetic)
            {
                if (lType != rType)
                {
                    throw new InvalidOperationException(
                        $"Mismatched types: {lExpr.Attributes.ExprType} and {rExpr.Attributes.ExprType} at {additiveExpr.Location}."
                    );
                }
                additiveExpr.Attributes.ExprType = lExpr.Attributes.ExprType;
            }
            else if (lType.IsIntegerLiteral && rType.IsIntegerLiteral)
            {
                additiveExpr.Attributes.ExprType = lType;
            }
            else if (lType.IsArithmetic && rType.IsIntegerLiteral)
            {
                rOp = (dynamic)ConvertArithmeticLiteralValue((ulong)rOp, (GSArithmeticType)lType);
                additiveExpr.Attributes.ExprType = lType;
            }
            else if (rType.IsArithmetic && lType.IsIntegerLiteral)
            {
                lOp = (dynamic)ConvertArithmeticLiteralValue((ulong)lOp, (GSArithmeticType)rType);
                additiveExpr.Attributes.ExprType = rType;
            }
            else
            {
                throw new InvalidOperationException($"At {additiveExpr.Location}: Invalid operator \'{op}\'.");
            }


            additiveExpr.Attributes.Value = (object)(
                    op == '+' ? lOp + rOp : lOp - rOp
                ) ?? throw new InternalErrorException($"Invalid \'{op}\' at {additiveExpr.Location}.");
        }

        void IVisitor.Visit(MultiplicativeExpr multiplicativeExpr)
        {
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
            if (lType.IsArithmetic && rType.IsArithmetic)
            {
                if (lType != rType)
                {
                    throw new InvalidOperationException(
                        $"Mismatched types: {lExpr.Attributes.ExprType} and {rExpr.Attributes.ExprType} at {multiplicativeExpr.Location}."
                    );
                }
                multiplicativeExpr.Attributes.ExprType = lExpr.Attributes.ExprType;
            }
            else if (lType.IsIntegerLiteral && rType.IsIntegerLiteral)
            {
                multiplicativeExpr.Attributes.ExprType = lType;
            }
            else if (lType.IsArithmetic && rType.IsIntegerLiteral)
            {
                rOp = (dynamic)ConvertArithmeticLiteralValue((ulong)rOp, (GSArithmeticType)lType);
                multiplicativeExpr.Attributes.ExprType = lType;
            }
            else if (rType.IsArithmetic && lType.IsIntegerLiteral)
            {
                lOp = (dynamic)ConvertArithmeticLiteralValue((ulong)lOp, (GSArithmeticType)rType);
                multiplicativeExpr.Attributes.ExprType = rType;
            }
            else
            {
                throw new InvalidOperationException($"At {multiplicativeExpr.Location}: Invalid operator \'{op}\'.");
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
            var operand = unaryExpr.Operand;
            operand.Accept(this);
            unaryExpr.Attributes.ExprType = operand.Attributes.ExprType;
            if (operand.Attributes.ExprType!.IsArithmetic)
            {
                unaryExpr.Attributes.Value = -(dynamic)operand.Attributes.Value!;
            }
            else if (operand.Attributes.ExprType!.IsIntegerLiteral)
            {
                unaryExpr.Attributes.Value = (ulong)(-(long)(ulong)operand.Attributes.Value!);
            }
            else
            {
                throw new InvalidOperationException($"At {unaryExpr.Location}: Invalid unary operator \'-\'.");
            }
        }

        void IVisitor.Visit(EmptyStmt emptyStmt)
        {
            emptyStmt.Attributes.StmtType = null;
        }

        void IVisitor.Visit(SingleStmt singleStmt)
        {
            singleStmt.Expr.Accept(this);
            if (singleStmt.Echo)
            {
                singleStmt.Attributes.StmtType = singleStmt.Expr.Attributes.ExprType;
                singleStmt.Attributes.Value = singleStmt.Expr.Attributes.Value;
            }
        }

        void IVisitor.Visit(IdExpr idExpr)
        {
            var rtti = this.scopeStack.LookUp(idExpr.Name);
            if (rtti is null)
            {
                throw new SymbolNotFoundException($"Unknown identifier \"{idExpr.Name}\" at {idExpr.Location}.");
            }

            idExpr.Attributes.ExprType = rtti.Type;
            idExpr.Attributes.Value = rtti.Value;
        }

        void IVisitor.Visit(IntegerLiteralExpr integerLiteralExpr)
        {
            integerLiteralExpr.Attributes.ExprType = GSIntegerLiteral.Instance;
            integerLiteralExpr.Attributes.Value = integerLiteralExpr.IntegerValue;
        }

        void IVisitor.Visit(BoolLiteralExpr boolLiteralExpr)
        {
            boolLiteralExpr.Attributes.ExprType = GSBoolLiteral.Instance;
            boolLiteralExpr.Attributes.Value = boolLiteralExpr.BoolValue;
        }

        void IVisitor.Visit(CompoundStmt compoundStmt)
        {
            this.scopeStack.OpenNewScope();
            var statements = compoundStmt.Statements;
            foreach (var statement in statements)
            {
                statement.Accept(this);
            }
            if (statements.Count > 0)
            {
                var lastStmt = statements.Last();
                compoundStmt.Attributes.StmtType = lastStmt.Attributes.StmtType;
                compoundStmt.Attributes.Value = lastStmt.Attributes.Value;
            }
            this.scopeStack.CloseScope();
        }
    }
}
