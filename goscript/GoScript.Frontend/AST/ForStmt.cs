using GoScript.Frontend.Runtime;
using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public sealed class ForStmt : Statement
    {
        public Statement? InitStmt { get; private init; }
        public Expression? Condition { get; private init; }
        public Statement? PostStmt { get; private init; }
        public CompoundStmt Statements { get; private init; }
        internal Scope? AttachedScope { get; set; }

        public ForStmt(CompoundStmt statements, SourceLocation location)
            : base(location)
        {
            this.Statements = statements;
        }

        public ForStmt(Expression condition, CompoundStmt statements, SourceLocation location)
            : base(location)
        {
            this.Condition = condition;
            this.Statements = statements;
        }

        public ForStmt(Statement initStmt, Expression condition, Statement postStmt, CompoundStmt statements, SourceLocation location)
            : base(location)
        {
            this.InitStmt = initStmt;
            this.Condition = condition;
            this.PostStmt = postStmt;
            this.Statements = statements;
        }

        public override string ToString()
        {
            string result = "for ";
            if (this.InitStmt is not null)
            {
                result += $"{this.InitStmt}; {this.Condition}; {this.PostStmt} ";
            }
            else if (this.Condition is not null)
            {
                result += $"{this.Condition} ";
            }
            return result + this.Statements;
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
