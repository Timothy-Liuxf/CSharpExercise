using GoScript.Frontend.AST;
using GoScript.Frontend.Runtime;
using GoScript.Frontend.Types;
using System.Reflection;

namespace GoScript.Frontend.Translation
{
    internal class Translator : IVisitor
    {
        private Scope scope = new();
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

        void IVisitor.Visit(VarDecl varDecl)
        {
            if (varDecl.InitExpr == null && varDecl.InitType == null)
            {
                throw new InternalErrorException($"At {varDecl.Location}: InitExpr and InitType shouldn't be null at the same time.");
            }
            if (this.scope.Symbols.ContainsKey(varDecl.VarName))
            {
                throw new ConflictException($"Conflict at {varDecl.Location}: the name \"{varDecl.VarName}\" has already defined.");
            }

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
            this.scope.Symbols[varDecl.VarName] = rtti;

            varDecl.Attributes.StmtType = new GSNilType();
            if (varDecl.InitExpr == null)
            {
                rtti.Value = Convert.ChangeType(0, ((GSBasicType)rtti.Type!).DotNetType);
            }
            else
            {
                varDecl.InitExpr.Accept(this);
                if (varDecl.InitType == null)
                {
                    rtti.Type = varDecl.InitExpr.Attributes.ExprType!;
                    rtti.Value = varDecl.InitExpr.Attributes.Value;
                }
                else
                {
                    rtti.Value = Convert.ChangeType(varDecl.InitExpr.Attributes.Value, ((GSBasicType)rtti.Type!).DotNetType);
                }
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
            if (lExpr.Attributes.ExprType is not GSBasicType
                || rExpr.Attributes.ExprType is not GSBasicType)
            {
                throw new InvalidOperationException($"At {additiveExpr.Location}: Invalid operator \'{op}\'.");
            }
            var commonType = GSBasicType.GetCommonType((GSBasicType)lExpr.Attributes.ExprType, (GSBasicType)rExpr.Attributes.ExprType);
            additiveExpr.Attributes.ExprType = commonType;
            additiveExpr.Attributes.Value = (object)(
                    op == '+' ?
                        (dynamic)Convert.ChangeType(lExpr.Attributes.Value, commonType.DotNetType)!
                        + (dynamic)Convert.ChangeType(rExpr.Attributes.Value, commonType.DotNetType)!
                    : (dynamic)Convert.ChangeType(lExpr.Attributes.Value, commonType.DotNetType)!
                        - (dynamic)Convert.ChangeType(rExpr.Attributes.Value, commonType.DotNetType)!
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
            if (lExpr.Attributes.ExprType is not GSBasicType
                || rExpr.Attributes.ExprType is not GSBasicType)
            {
                throw new InvalidOperationException($"At {multiplicativeExpr.Location}: Invalid operator \'{op}\'.");
            }
            var commonType = GSBasicType.GetCommonType((GSBasicType)lExpr.Attributes.ExprType, (GSBasicType)rExpr.Attributes.ExprType);
            var lOp = (dynamic)Convert.ChangeType(lExpr.Attributes.Value, commonType.DotNetType)!;
            var rOp = (dynamic)Convert.ChangeType(rExpr.Attributes.Value, commonType.DotNetType)!;
            multiplicativeExpr.Attributes.ExprType = commonType;
            multiplicativeExpr.Attributes.Value = (object)(op switch
            {
                '*' => lOp * rOp,
                '/' => lOp / rOp,
                _ => lOp % rOp,
            });
        }

        void IVisitor.Visit(UnaryExpr unaryExpr)
        {
            var operand = unaryExpr.Operand;
            operand.Accept(this);
            if (operand.Attributes.ExprType is not GSBasicType)
            {
                throw new InvalidOperationException($"At {unaryExpr.Location}: Invalid unary operator \'-\'.");
            }
            unaryExpr.Attributes.ExprType = operand.Attributes.ExprType;
            unaryExpr.Attributes.Value = -(dynamic)operand.Attributes.Value!;
        }

        void IVisitor.Visit(EmptyStmt emptyStmt)
        {
            emptyStmt.Attributes.StmtType = new GSNilType();
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
            if (!this.scope.Symbols.ContainsKey(idExpr.Name))
            {
                throw new SymbolNotFoundException($"Unknown identifier \"{idExpr.Name}\" at {idExpr.Location}.");
            }

            var rtti = this.scope.Symbols[idExpr.Name];
            idExpr.Attributes.ExprType = rtti.Type;
            idExpr.Attributes.Value = rtti.Value;
        }

        void IVisitor.Visit(IntegerRValueExpr integerRValueExpr)
        {
            integerRValueExpr.Attributes.ExprType = new GSInt32();
            integerRValueExpr.Attributes.Value = (int)integerRValueExpr.IntegerValue;
        }
    }
}
