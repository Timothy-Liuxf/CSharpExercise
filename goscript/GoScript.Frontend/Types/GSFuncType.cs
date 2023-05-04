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
        public IReadOnlyList<GSType> ReturnType { get; private init; }
        public IReadOnlyList<GSType> ParamType { get; private init; }

        public override bool EqualsImpl(GSType other)
        {
            return other.IsFunc
                && this.ReturnType.Count == (other as GSFuncType)!.ReturnType.Count
                && this.ParamType.Count == (other as GSFuncType)!.ParamType.Count
                && this.ReturnType.SequenceEqual((other as GSFuncType)!.ReturnType)
                && this.ParamType.SequenceEqual((other as GSFuncType)!.ParamType);
        }

        public override int GetHashCodeImpl()
        {
            int hash = 0;
            foreach (var type in this.ReturnType)
            {
                hash ^= type.GetHashCode();
            }
            foreach (var type in this.ParamType)
            {
                hash ^= type.GetHashCode();
            }
            return hash;
        }

        public GSFuncType(IReadOnlyList<GSType> returnType, IReadOnlyList<GSType> paramType)
        {
            // Create new lists
            this.ReturnType = returnType.ToList();
            this.ParamType = paramType.ToList();
        }
    }
}
