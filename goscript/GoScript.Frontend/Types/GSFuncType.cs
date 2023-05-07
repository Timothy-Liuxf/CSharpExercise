using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoScript.Frontend.Types
{
    public sealed class GSFuncType : GSType
    {
        public override bool IsFunc => true;
        public IReadOnlyList<GSType> ParamType { get; private init; }
        public IReadOnlyList<GSType> ReturnType { get; private init; }

        public override bool EqualsImpl(GSType other)
        {
            return other.IsFunc
                && this.ParamType.Count == (other as GSFuncType)!.ParamType.Count
                && this.ReturnType.Count == (other as GSFuncType)!.ReturnType.Count
                && this.ParamType.SequenceEqual((other as GSFuncType)!.ParamType)
                && this.ReturnType.SequenceEqual((other as GSFuncType)!.ReturnType);
        }

        public override int GetHashCodeImpl()
        {
            int hash = 0;
            foreach (var type in this.ParamType)
            {
                hash ^= type.GetHashCode();
            }
            foreach (var type in this.ReturnType)
            {
                hash ^= type.GetHashCode();
            }
            return hash;
        }

        public GSFuncType(IReadOnlyList<GSType> paramType, IReadOnlyList<GSType> returnType)
        {
            // Create new lists
            this.ParamType = paramType.ToList();
            this.ReturnType = returnType.ToList();
        }

        public override string ToString()
        {
            var result = new StringBuilder("func (");
            if (this.ParamType.Count > 0)
            {
                result.Append(this.ParamType[0]);
                for (int i = 1; i < this.ParamType.Count; i++)
                {
                    result.Append(", ");
                    result.Append(this.ParamType[i]);
                }
            }
            result.Append(")");

            if (this.ReturnType.Count > 0)
            {
                result.Append(" ");
                if (this.ReturnType.Count == 1)
                {
                    result.Append(this.ReturnType[0]);
                }
                else
                {
                    result.Append("(");
                    result.Append(this.ReturnType[0]);
                    for (int i = 1; i < this.ReturnType.Count; i++)
                    {
                        result.Append(", ");
                        result.Append(this.ReturnType[i]);
                    }
                    result.Append(")");
                }
            }
            return result.ToString();
        }
    }
}
