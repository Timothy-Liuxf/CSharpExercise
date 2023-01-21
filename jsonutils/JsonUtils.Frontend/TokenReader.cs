namespace JsonUtils.Frontend
{
    internal class TokenReader
    {
        private IEnumerator<Token> tokenItr;

        public Token? CurrentToken { get; private set; }

        public bool NextToken()
        {
            if (tokenItr.MoveNext())
            {
                CurrentToken = tokenItr.Current;
                return true;
            }
            else
            {
                CurrentToken = null;
                return false;
            }
        }

        public TokenReader(IList<Token> tokens)
        {
            tokenItr = tokens.GetEnumerator();
        }
    }
}
