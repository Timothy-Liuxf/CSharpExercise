using GoScript.Frontend.Types;
using GoScript.Utils;
using System.Linq;
using System.Text;

namespace GoScript.Frontend.AST
{
    public sealed class FuncExpr : Expression
    {
        public Compound Body { get; private init; }

        public IReadOnlyList<(GSType, string)> Params { get; private init; }

        public IReadOnlyList<GSType> ReturnTypes { get; private init; }

        public FuncExpr(Compound body, IReadOnlyList<(GSType, string)> parameters,
            IReadOnlyList<GSType> returnTypes, SourceLocation location)
            : base(location)
        {
            this.Body = body;
            this.Params = parameters.ToList();
            this.ReturnTypes = returnTypes.ToList();
        }

        public override string ToString()
        {
            var result = new StringBuilder("func (");
            result.Append(string.Join(", ", this.Params.Select((@param, _) => $"{@param.Item2} {@param.Item1}")));
            result.Append(") ");
            if (this.ReturnTypes.Count == 1)
            {
                result.Append(this.ReturnTypes[0] + " ");
            }
            else if (this.ReturnTypes.Count > 1)
            {
                result.Append("(");
                result.Append(string.Join(", ", this.ReturnTypes));
                result.Append(") ");
            }
            result.Append(this.Body.ToString());
            return result.ToString();
        }

        internal override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
