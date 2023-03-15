namespace GoScript.Frontend.Types
{
    public abstract class GSBasicType : GSType
    {
        public abstract Type DotNetType { get; }
    }

    public sealed class GSInt32 : GSBasicType
    {
        public override Type DotNetType => typeof(int);
    }
}
