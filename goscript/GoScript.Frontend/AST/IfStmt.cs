using GoScript.Frontend.Runtime;
using GoScript.Utils;

namespace GoScript.Frontend.AST
{
    public sealed class IfStmt : Statement
    {
        public IList<(Expression, Compound, SourceLocation)> ConditionBranches { get; private init; }
        public Compound? ElseBranch { get; private init; }

        public SourceLocation Location { get; private init; }

        public IfStmt(IList<(Expression, Compound, SourceLocation)> conditionBranches, SourceLocation location)
        {
            this.ConditionBranches = conditionBranches;
            this.Location = location;
        }

        public IfStmt(IList<(Expression, Compound, SourceLocation)> conditionBranches, Compound elseBranch, SourceLocation location)
        {
            this.ConditionBranches = conditionBranches;
            this.ElseBranch = elseBranch;
            this.Location = location;
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
            return result;
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
