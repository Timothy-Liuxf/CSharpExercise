namespace JsonUtils.Frontend
{
    internal class TokenReader
    {
        private IEnumerator<Token> tokenItr;
        public SourceLocation EndLocation { get; private init; }
        public Token? CurrentToken { get; private set; }

        private Token? AssertCurrentTokenHelper(bool ignoreComment)
        {
            if (!ignoreComment) return this.CurrentToken;

            var currToken = this.CurrentToken;
            while (currToken is not null)
            {
                if (currToken.Type != TokenType.LineComment)
                {
                    return currToken;
                }
                this.NextToken();
                currToken = this.CurrentToken;
            }
            return null;
        }

        public Token AssertCurrentTokenNotNull(string message, bool ignoreComment = true)
        {
            return AssertCurrentTokenHelper(ignoreComment) ?? throw new SyntaxErrorException(EndLocation, message);
        }

        public void AssertNullToken(string message, bool ignoreComment = true)
        {
            var currToken = AssertCurrentTokenHelper(ignoreComment);
            if (currToken is not null)
            {
                throw new SyntaxErrorException(currToken.Location, message);
            }
        }

        public Token MatchToken(TokenType token, string expected, bool ignoreComment = true)
        {
            if (token == TokenType.LineComment)
            {
                throw new ArgumentException($"Cannot match comment token in {nameof(MatchToken)}.");
            }

            Token currToken;
            if (ignoreComment)
            {
                while (true)
                {
                    currToken = AssertCurrentTokenNotNull($"Missing {expected} token.");
                    if (currToken.Type == TokenType.LineComment)
                    {
                        continue;
                    }
                    break;
                }
            }
            else
            {
                currToken = AssertCurrentTokenNotNull($"Missing {expected} token.");
            }
            if (currToken.Type != token)
            {
                throw new SyntaxErrorException(currToken.Location, $"Expect {expected} token, found {currToken}.");
            }
            this.NextToken();
            return currToken;
        }

        public void MatchToken(bool ignoreComment = true)
        {
            if (ignoreComment)
            {
                while (true)
                {
                    var currToken = AssertCurrentTokenNotNull($"Missing token.");
                    if (currToken.Type == TokenType.LineComment)
                    {
                        continue;
                    }
                    this.NextToken();
                }
            }
            else
            {
                AssertCurrentTokenNotNull($"Missing token.");
                this.NextToken();
            }
        }

        public Token PeekToken(bool ignoreComment = true)
        {
            if (ignoreComment)
            {
                while (true)
                {
                    var currToken = AssertCurrentTokenNotNull($"Missing token.");
                    if (currToken.Type == TokenType.LineComment)
                    {
                        continue;
                    }
                    return currToken;
                }
            }
            else
            {
                return AssertCurrentTokenNotNull($"Missing token.");
            }
        }

        private bool NextToken()
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

        public TokenReader(IList<Token> tokens, SourceLocation endLocation)
        {
            tokenItr = tokens.GetEnumerator();
            EndLocation = endLocation;
            this.NextToken();
        }
    }
}
