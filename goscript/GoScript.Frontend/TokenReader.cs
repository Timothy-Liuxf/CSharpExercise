namespace GoScript.Frontend
{
    internal class TokenReader
    {
        public Token? CurrentToken { get; private set; }

        public bool NextToken()
        {
            if (tokenItr.MoveNext())
            {
                this.CurrentToken = tokenItr.Current;
                return true;
            }
            else
            {
                this.CurrentToken = null;
                return false;
            }
        }

        public Token AssertCurrentTokenNotNull(string message)
        {
            return this.CurrentToken ?? throw new SyntaxErrorException(message);
        }

        public Token AssertCurrentTokenNotNull()
        {
            return this.CurrentToken ?? throw new SyntaxErrorException("Missing token.");
        }

        public Punctuator MatchPunctuator(PunctuatorType type)
        {
            var token = AssertCurrentTokenNotNull($"Expect token: \'{Punctuator.GetPunctuator(type)}\'.");
            if (token.TokenCatagory == TokenType.Punctuator)
            {
                var punctuator = (Punctuator)token;
                if (punctuator.Type == type)
                {
                    this.NextToken();
                    return punctuator;
                }
            }
            throw new SyntaxErrorException(token.Location, $"Expect token \'{Punctuator.GetPunctuator(type)}\', found \'{token.ToString()}\'.");
        }

        public bool TryPeekPunctuator(PunctuatorType type, out Punctuator? punctuator)
        {
            var token = this.PeekToken();
            if (token.TokenCatagory == TokenType.Punctuator
                && ((Punctuator)token).Type == type)
            {
                punctuator = (Punctuator)token;
                return true;
            }
            punctuator = null;
            return false;
        }

        public bool TryMatchPunctuator(PunctuatorType type, out Punctuator? punctuator)
        {
            if (TryPeekPunctuator(type, out punctuator))
            {
                this.MatchToken();
                return true;
            }
            return false;
        }

        public Keyword MatchKeyword(KeywordType type)
        {
            var token = AssertCurrentTokenNotNull($"Expect token: \'{Keyword.GetKeywordString(type)}\'.");
            if (token.TokenCatagory == TokenType.Keyword)
            {
                var keyword = (Keyword)token;
                if (keyword.Type == type)
                {
                    this.NextToken();
                    return keyword;
                }
            }
            throw new SyntaxErrorException(token.Location, $"Expect token \'{Keyword.GetKeywordString(type)}\', found \'{token}\'.");
        }

        public bool TryPeekKeyword(KeywordType type, out Keyword? keyword)
        {
            var token = this.PeekToken();
            if (token.TokenCatagory == TokenType.Keyword
                && ((Keyword)token).Type == type)
            {
                keyword = (Keyword)token;
                return true;
            }
            keyword = null;
            return false;
        }

        public bool TryMatchKeyword(KeywordType type, out Keyword? keyword)
        {
            if (TryPeekKeyword(type, out keyword))
            {
                return true;
            }
            return false;
        }

        public bool TryPeekTypeKeyword(out Keyword? keyword)
        {
            var token = this.PeekToken();
            if (token.TokenCatagory == TokenType.Keyword
                && ((Keyword)token).IsTypeKeyword())
            {
                keyword = (Keyword)token;
                return true;
            }
            keyword = null;
            return false;
        }

        public bool TryMatchTypeKeyword(out Keyword? keyword)
        {
            if (TryPeekTypeKeyword(out keyword))
            {
                this.NextToken();
                return true;
            }
            return false;
        }

        public Literal MatchLiteral(LiteralType type)
        {
            var token = AssertCurrentTokenNotNull($"Expect {type.ToString()}.");
            if (token.TokenCatagory == TokenType.Literal)
            {
                var literal = (Literal)token;
                if (literal.Type == type)
                {
                    this.NextToken();
                    return literal;
                }
            }
            throw new SyntaxErrorException(token.Location, $"Expect {type.ToString()}, found \'{token.ToString()}\'.");
        }

        public bool TryPeekLiteral(LiteralType type, out Literal? literal)
        {
            var token = this.PeekToken();
            if (token.TokenCatagory == TokenType.Literal
                && ((Literal)token).Type == type)
            {
                literal = (Literal)token;
                return true;
            }
            literal = null;
            return false;
        }

        public bool TryMatchLiteral(LiteralType type, out Literal? literal)
        {
            if (TryPeekLiteral(type, out literal))
            {
                this.MatchToken();
                return true;
            }
            return false;
        }

        public Identifier MatchIdentifier()
        {
            var token = AssertCurrentTokenNotNull($"Expect identifier.");
            if (token.TokenCatagory == TokenType.Identifier)
            {
                this.NextToken();
                return (Identifier)token;
            }
            throw new SyntaxErrorException(token.Location, $"Expect identifier, found \'{token.ToString()}\'.");
        }

        public bool TryPeekIdentifier(out Identifier? identifier)
        {
            var token = this.PeekToken();
            if (token.TokenCatagory == TokenType.Identifier)
            {
                identifier = (Identifier)token;
                return true;
            }
            identifier = null;
            return false;
        }

        public bool TryMatchIdentifier(out Identifier? identifier)
        {
            if (TryPeekIdentifier(out identifier))
            {
                this.MatchToken();
                return true;
            }
            return false;
        }

        public Newline MatchNewline()
        {
            var token = AssertCurrentTokenNotNull($"Expect newline character.");
            if (token.TokenCatagory == TokenType.Newline)
            {
                this.NextToken();
                return (Newline)token;
            }
            throw new SyntaxErrorException(token.Location, $"Expect newline character, found \'{token.ToString()}\'.");
        }

        public bool TryPeekNewline(out Newline? newline)
        {
            var token = this.PeekToken();
            if (token.TokenCatagory == TokenType.Newline)
            {
                newline = (Newline)token;
                return true;
            }
            newline = null;
            return false;
        }

        public bool TryMatchNewline(out Newline? newline)
        {
            if (TryPeekNewline(out newline))
            {
                this.MatchToken();
                return true;
            }
            return false;
        }

        public Token MatchToken(string? message = null)
        {
            var token = AssertCurrentTokenNotNull(message ?? $"Missing token.");
            this.NextToken();
            return token;
        }

        public Token PeekToken(string? message = null)
        {
            return AssertCurrentTokenNotNull(message ?? $"Missing token.");
        }

        private readonly IEnumerator<Token> tokenItr;

        public TokenReader(IEnumerable<Token> tokens)
        {
            this.tokenItr = tokens.GetEnumerator();
            this.NextToken();
        }
    }
}
