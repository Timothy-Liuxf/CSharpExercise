using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public sealed class IfStmt : Statement
    {
        public IList<(Expression, Compound, SourceLocation)> ConditionBranches { get; private init; }
        public Compound? ElseBranch { get; private init; }

        public IfStmt(IList<(Expression, Compound, SourceLocation)> conditionBranches, SourceLocation location)
            : base(location)
        {
            this.ConditionBranches = conditionBranches;
        }

        public IfStmt(IList<(Expression, Compound, SourceLocation)> conditionBranches, Compound elseBranch, SourceLocation location)
            : base(location)
        {
            this.ConditionBranches = conditionBranches;
            this.ElseBranch = elseBranch;
        }

        public override string ToString()
        {
            bool first = true;
            string result = "";
            foreach (var (condition, branch, _) in this.ConditionBranches)
            {
                result += (first ? "if" : " else if") + $" {condition} {branch}";
                first = false;
            }
            if (this.ElseBranch is not null)
            {
                result += $" else {this.ElseBranch}" + Environment.NewLine;
            }
            else
            {
                result += Environment.NewLine;
            }
            return result;
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
