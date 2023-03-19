namespace GoScript.Frontend.Runtime
{
    internal class Scope
    {
        public IDictionary<string, RTTI> Symbols = new Dictionary<string, RTTI>();

        public void ClearValues()
        {
            foreach (var (_, rtti) in this.Symbols)
            {
                rtti.Value = null;
            }
        }
    }
}
