using GoScript.Utils;
using System.Security.Cryptography;

namespace GoScript.Frontend.AST
{
    public sealed class UnaryExpr : Expression
    {
        public enum OperatorType
        {
            Neg,
            Not,
        }

        public OperatorType Operator { get; private init; }
        public Expression Operand { get; private init; }
        public SourceLocation Location { get; private init; }

        public UnaryExpr(OperatorType @operator, Expression operand, SourceLocation location)
        {
            this.Operator = @operator;
            this.Operand = operand;
            this.Location = location;
        }

        public override string ToString()
        {
            return $"-{this.Operand}";
        }

        internal override void Accept(IVisitor visitor) => visitor.Visit(this);
    }
}
