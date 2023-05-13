using GoScript.Frontend.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoScript.Frontend.Runtime
{
    public class FuncValue
    {
        internal FuncExpr? FuncExpr { get; private set; }

        public FuncValue(FuncExpr? funcExpr)
        {
            this.FuncExpr = funcExpr;
        }

        public override string ToString()
        {
            return FuncExpr is null ? "<nil>" : "<function>";
        }
    }
}
