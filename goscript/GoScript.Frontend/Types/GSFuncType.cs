using GoScript.Utils;
using System.Text;

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

        public GSFuncType(IEnumerable<GSType> paramType,
            IEnumerable<GSType> returnType)
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
                result.Append(string.Join(", ", this.ParamType));
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
                    result.Append("(" + string.Join(", ", this.ReturnType) + ")");
                }
            }
            return result.ToString();
        }
    }
}
