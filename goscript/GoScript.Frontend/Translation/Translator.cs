using GoScript.Frontend.AST;
using GoScript.Frontend.Runtime;
using GoScript.Frontend.Types;

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
                ast.Accept(this);
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
                if (varDecl.InitType == "int32")
                {
                    rtti.type = new GSInt32();
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
                rtti.value = (int)0;
            }
            else
            {
                varDecl.InitExpr.Accept(this);
                if (varDecl.InitType == null)
                {
                    rtti.type = varDecl.InitExpr.Attributes.ExprType!;
                    rtti.value = varDecl.InitExpr.Attributes.Value;
                }
                else
                {
                    rtti.value = Convert.ChangeType(varDecl.InitExpr.Attributes.Value, ((GSBasicType)rtti.type).DotNetType);
                }
            }
        }

        void IVisitor.Visit(AddExpr addExpr)
        {
            var lExpr = addExpr.LExpr;
            var rExpr = addExpr.RExpr;
            lExpr.Accept(this);
            rExpr.Accept(this);
            if (lExpr.Attributes.ExprType is not GSInt32
                || rExpr.Attributes.ExprType is not GSInt32)
            {
                throw new InvalidOperationException($"At {addExpr.Location}: Invalid operator \'+\'.");
            }
            addExpr.Attributes.ExprType = new GSInt32();
            addExpr.Attributes.Value = (int)lExpr.Attributes.Value! + (int)rExpr.Attributes.Value!;
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
            idExpr.Attributes.ExprType = rtti.type;
            idExpr.Attributes.Value = rtti.value;
        }

        void IVisitor.Visit(IntegerRValueExpr integerRValueExpr)
        {
            integerRValueExpr.Attributes.ExprType = new GSInt32();
            integerRValueExpr.Attributes.Value = (int)integerRValueExpr.IntegerValue;
        }
    }
}
