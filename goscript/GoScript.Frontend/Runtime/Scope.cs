namespace GoScript.Frontend.Runtime
{
    internal class Scope
    {
        public IDictionary<string, RTTI> Symbols = new Dictionary<string, RTTI>();
    }
}
