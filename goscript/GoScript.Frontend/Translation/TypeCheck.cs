using GoScript.Frontend.AST;
using GoScript.Frontend.Runtime;
using GoScript.Frontend.Types;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace GoScript.Frontend.Translation
{
    /// <summary>
    /// Check type validation, build symbols and calculate the value of constant expressions.
    /// </summary>
    internal class TypeCheck : IVisitor
    {
        private readonly ScopeStack scopeStack = new();

        public TypeCheck(ScopeStack scopeStack)
        {
            this.scopeStack = scopeStack;
        }

        private static bool CheckArithmeticConvertible(ulong constantValue, GSArithmeticType targetType)
        {
            var getStaticFieldValue = (string name) => targetType.DotNetType.GetField(name, BindingFlags.Public | BindingFlags.Static)!.GetValue(null);
            dynamic maxValue = getStaticFieldValue("MaxValue");
            dynamic minValue = getStaticFieldValue("MinValue");
            if (targetType.IsSigned)
            {
                dynamic signValue = (long)constantValue;
                return signValue >= minValue && signValue <= maxValue;
            }
            else
            {
                return constantValue >= minValue && constantValue <= maxValue;
            }
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
                if (varDecl.InitExprs is not null)
                {
                    var initExpr = varDecl.InitExprs[i];
                    var varName = varDecl.VarNames[i];
                    initExpr.Accept(this);
                    var exprType = initExpr.Attributes.ExprType!;
                    if (varDecl.InitType == null)
                    {
                        if (exprType.IsIntegerConstant)
                        {
                            var type = GSInt64.Instance;
                            rtti.Type = type;
                            if (!CheckArithmeticConvertible((ulong)initExpr.Attributes.Value!, type))
                            {
                                throw new InvalidOperationException($"At {varDecl.Location}: The value {initExpr.Attributes.Value} is out of range of int.");
                            }
                        }
                        else if (exprType.IsBoolConstant)
                        {
                            rtti.Type = GSBool.Instance;
                        }
                        else
                        {
                            rtti.Type = exprType;
                        }
                    }
                    else
                    {
                        if (exprType.IsIntegerConstant)
                        {
                            if (rtti.Type!.IsArithmetic)
                            {
                                if (!CheckArithmeticConvertible((ulong)initExpr.Attributes.Value!, (GSArithmeticType)rtti.Type))
                                {
                                    throw new InvalidOperationException($"At {varDecl.Location}: The value {initExpr.Attributes.Value} is out of range of {rtti.Type}.");
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException(
                                    $"At {varDecl.Location}: Cannot use integer constant to init {rtti.Type} variable \"{varName}\".");
                            }
                        }
                        else if (exprType.IsBoolConstant)
                        {
                            if (!rtti.Type!.IsBool)
                            {
                                throw new InvalidOperationException(
                                    $"At {varDecl.Location}: Cannot use bool constant {initExpr} to init {rtti.Type} variable \"{varName}\".");
                            }
                        }
                        else
                        {
                            if (rtti.Type! != exprType)
                            {
                                throw new InvalidOperationException($"Mismatched type {rtti.Type} and {exprType} at {varDecl.Location}.");
                            }
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
            else if (lType.IsIntegerConstant && rType.IsIntegerConstant)
            {
                additiveExpr.Attributes.ExprType = lType;
                additiveExpr.Attributes.Value = (object)(
                    op == '+' ? (ulong)lOp + (ulong)rOp : (ulong)lOp - (ulong)rOp
                ) ?? throw new InternalErrorException($"Invalid \'{op}\' at {additiveExpr.Location}.");
                additiveExpr.IsConstantEvaluated = true;
            }
            else if (lType.IsArithmetic && rType.IsIntegerConstant)
            {
                if (!CheckArithmeticConvertible((ulong)rOp, (GSArithmeticType)lType))
                {
                    throw new InvalidOperationException($"At {additiveExpr.Location}: The value {rOp} is out of range of {lType}.");
                }
                additiveExpr.Attributes.ExprType = lType;
            }
            else if (rType.IsArithmetic && lType.IsIntegerConstant)
            {
                if (!CheckArithmeticConvertible((ulong)lOp, (GSArithmeticType)rType))
                {
                    throw new InvalidOperationException($"At {additiveExpr.Location}: The value {lOp} is out of range of {rType}.");
                }
                additiveExpr.Attributes.ExprType = rType;
            }
            else
            {
                throw new InvalidOperationException($"At {additiveExpr.Location}: Invalid operator \'{op}\'.");
            }
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
            else if (lType.IsIntegerConstant && rType.IsIntegerConstant)
            {
                var lConstant = (ulong)lOp;
                var rConstant = (ulong)rOp;
                multiplicativeExpr.Attributes.ExprType = lType;
                multiplicativeExpr.Attributes.Value = (op switch
                {
                    '*' => lConstant * rConstant,
                    '/' => lConstant / rConstant,
                    _ => lConstant % rConstant,
                });
                multiplicativeExpr.IsConstantEvaluated = true;
            }
            else if (lType.IsArithmetic && rType.IsIntegerConstant)
            {
                if (!CheckArithmeticConvertible((ulong)rOp, (GSArithmeticType)lType))
                {
                    throw new InvalidOperationException($"At {multiplicativeExpr.Location}: The value {rOp} is out of range of {lType}.");
                }
                multiplicativeExpr.Attributes.ExprType = lType;
            }
            else if (rType.IsArithmetic && lType.IsIntegerConstant)
            {
                if (!CheckArithmeticConvertible((ulong)lOp, (GSArithmeticType)rType))
                {
                    throw new InvalidOperationException($"At {multiplicativeExpr.Location}: The value {lOp} is out of range of {rType}.");
                }
                multiplicativeExpr.Attributes.ExprType = rType;
            }
            else
            {
                throw new InvalidOperationException($"At {multiplicativeExpr.Location}: Invalid operator \'{op}\'.");
            }
        }

        void IVisitor.Visit(UnaryExpr unaryExpr)
        {
            var operand = unaryExpr.Operand;
            operand.Accept(this);
            unaryExpr.Attributes.ExprType = operand.Attributes.ExprType;
            var exprType = unaryExpr.Attributes.ExprType!;
            var operandValue = operand.Attributes.Value!;

            switch (unaryExpr.Operator)
            {
                case UnaryExpr.OperatorType.Neg:
                    if (exprType.IsIntegerConstant)
                    {
                        unaryExpr.Attributes.Value = (ulong)(-(long)(ulong)operandValue);
                        unaryExpr.IsConstantEvaluated = true;
                    }
                    else if (!exprType.IsArithmetic)
                    {
                        throw new InvalidOperationException($"At {unaryExpr.Location}: Invalid unary operator \'-\' on type {exprType}.");
                    }
                    break;
                case UnaryExpr.OperatorType.Not:
                    if (exprType.IsBoolConstant)
                    {
                        unaryExpr.Attributes.Value = !(bool)operandValue;
                        unaryExpr.IsConstantEvaluated = true;
                    }
                    else if (!exprType.IsBool)
                    {
                        throw new InvalidOperationException($"At {unaryExpr.Location}: Invalid unary operator \'!\' on type {exprType}.");
                    }
                    break;
                default:
                    throw new InternalErrorException($"At {unaryExpr.Location}: Invalid operator type {unaryExpr.Operator}.");
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
        }

        void IVisitor.Visit(IntegerConstantExpr integerConstantExpr)
        {
            integerConstantExpr.Attributes.ExprType = GSIntegerConstant.Instance;
            integerConstantExpr.Attributes.Value = integerConstantExpr.IntegerValue;
            integerConstantExpr.IsConstantEvaluated = true;
        }

        void IVisitor.Visit(BoolConstantExpr boolConstantExpr)
        {
            boolConstantExpr.Attributes.ExprType = GSBoolConstant.Instance;
            boolConstantExpr.Attributes.Value = boolConstantExpr.BoolValue;
            boolConstantExpr.IsConstantEvaluated = true;
        }

        void IVisitor.Visit(CompoundStmt compoundStmt)
        {
            var scope = new Scope();
            compoundStmt.AttachedScope = scope;
            this.scopeStack.AttachScope(scope);
            var statements = compoundStmt.Statements;
            foreach (var statement in statements)
            {
                statement.Accept(this);
            }
            if (statements.Count > 0)
            {
                var lastStmt = statements.Last();
                compoundStmt.Attributes.StmtType = lastStmt.Attributes.StmtType;
            }
            this.scopeStack.CloseScope();
        }
    }
}
