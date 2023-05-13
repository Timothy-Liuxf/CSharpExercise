using GoScript.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoScript.Frontend.AST
{
    public class ReturnStmt : Statement
    {
        public IReadOnlyList<Expression> ReturnExpr { get; private init; }

        public ReturnStmt(IEnumerable<Expression> returnExpr, SourceLocation location)
            : base(location)
        {
            this.ReturnExpr = returnExpr.ToList();
        }

        public override string ToString()
        {
            return "return " + string.Join(", ", ReturnExpr) + Environment.NewLine;
        }

        internal override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
