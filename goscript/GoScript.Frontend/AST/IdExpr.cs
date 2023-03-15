using GoScript.Frontend.Types;

namespace GoScript.Frontend.AST
{
    public sealed class IdExpr : Expression
    {
        public override object? Value { get; internal set; }

        public override GSType? ExprType { get; internal set; }

        public string Name { get; private init; }
        public IdExpr(string name)
        {
            this.Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
