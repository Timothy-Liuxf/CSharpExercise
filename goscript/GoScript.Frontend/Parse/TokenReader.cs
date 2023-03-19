using GoScript.Frontend.Lex;
using System.Diagnostics.CodeAnalysis;

namespace GoScript.Frontend.Parse
{
    internal class TokenReader
    {
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

        public Token AssertCurrentTokenNotNull(string message)
        {
            return CurrentToken ?? throw new SyntaxErrorException(message);
        }

        public Token AssertCurrentTokenNotNull()
        {
            return CurrentToken ?? throw new SyntaxErrorException("Missing token.");
        }

        public Punctuator MatchPunctuator(PunctuatorType type)
        {
            var token = AssertCurrentTokenNotNull($"Expect token: \'{Punctuator.GetPunctuator(type)}\'.");
            if (token.TokenCatagory == TokenType.Punctuator)
            {
                var punctuator = (Punctuator)token;
                if (punctuator.Type == type)
                {
                    NextToken();
                    return punctuator;
                }
            }
            throw new SyntaxErrorException(token.Location, $"Expect token \'{Punctuator.GetPunctuator(type)}\', found \'{token.ToString()}\'.");
        }

        public bool TryPeekPunctuator(PunctuatorType type, [MaybeNullWhen(false), NotNullWhen(true)] out Punctuator? punctuator)
        {
            var token = PeekToken();
            if (token.TokenCatagory == TokenType.Punctuator
                && ((Punctuator)token).Type == type)
            {
                punctuator = (Punctuator)token;
                return true;
            }
            punctuator = null;
            return false;
        }

        public bool TryMatchPunctuator(PunctuatorType type, [MaybeNullWhen(false), NotNullWhen(true)] out Punctuator? punctuator)
        {
            if (TryPeekPunctuator(type, out punctuator))
            {
                MatchToken();
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
                    NextToken();
                    return keyword;
                }
            }
            throw new SyntaxErrorException(token.Location, $"Expect token \'{Keyword.GetKeywordString(type)}\', found \'{token}\'.");
        }

        public bool TryPeekKeyword(KeywordType type, [MaybeNullWhen(false), NotNullWhen(true)] out Keyword? keyword)
        {
            var token = PeekToken();
            if (token.TokenCatagory == TokenType.Keyword
                && ((Keyword)token).Type == type)
            {
                keyword = (Keyword)token;
                return true;
            }
            keyword = null;
            return false;
        }

        public bool TryMatchKeyword(KeywordType type, [MaybeNullWhen(false), NotNullWhen(true)] out Keyword? keyword)
        {
            if (TryPeekKeyword(type, out keyword))
            {
                return true;
            }
            return false;
        }

        public bool TryPeekTypeKeyword([MaybeNullWhen(false), NotNullWhen(true)] out Keyword? keyword)
        {
            var token = PeekToken();
            if (token.TokenCatagory == TokenType.Keyword
                && ((Keyword)token).IsTypeKeyword())
            {
                keyword = (Keyword)token;
                return true;
            }
            keyword = null;
            return false;
        }

        public bool TryMatchTypeKeyword([MaybeNullWhen(false), NotNullWhen(true)] out Keyword? keyword)
        {
            if (TryPeekTypeKeyword(out keyword))
            {
                NextToken();
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
                    NextToken();
                    return literal;
                }
            }
            throw new SyntaxErrorException(token.Location, $"Expect {type.ToString()}, found \'{token.ToString()}\'.");
        }

        public bool TryPeekLiteral(LiteralType type, [MaybeNullWhen(false), NotNullWhen(true)] out Literal? literal)
        {
            var token = PeekToken();
            if (token.TokenCatagory == TokenType.Literal
                && ((Literal)token).Type == type)
            {
                literal = (Literal)token;
                return true;
            }
            literal = null;
            return false;
        }

        public bool TryMatchLiteral(LiteralType type, [MaybeNullWhen(false), NotNullWhen(true)] out Literal? literal)
        {
            if (TryPeekLiteral(type, out literal))
            {
                MatchToken();
                return true;
            }
            return false;
        }

        public Identifier MatchIdentifier()
        {
            var token = AssertCurrentTokenNotNull($"Expect identifier.");
            if (token.TokenCatagory == TokenType.Identifier)
            {
                NextToken();
                return (Identifier)token;
            }
            throw new SyntaxErrorException(token.Location, $"Expect identifier, found \'{token.ToString()}\'.");
        }

        public bool TryPeekIdentifier([MaybeNullWhen(false), NotNullWhen(true)] out Identifier? identifier)
        {
            var token = PeekToken();
            if (token.TokenCatagory == TokenType.Identifier)
            {
                identifier = (Identifier)token;
                return true;
            }
            identifier = null;
            return false;
        }

        public bool TryMatchIdentifier([MaybeNullWhen(false), NotNullWhen(true)] out Identifier? identifier)
        {
            if (TryPeekIdentifier(out identifier))
            {
                MatchToken();
                return true;
            }
            return false;
        }

        public Newline MatchNewline()
        {
            var token = AssertCurrentTokenNotNull($"Expect newline character.");
            if (token.TokenCatagory == TokenType.Newline)
            {
                NextToken();
                return (Newline)token;
            }
            throw new SyntaxErrorException(token.Location, $"Expect newline character, found \'{token.ToString()}\'.");
        }

        public bool TryPeekNewline([MaybeNullWhen(false), NotNullWhen(true)] out Newline? newline)
        {
            var token = PeekToken();
            if (token.TokenCatagory == TokenType.Newline)
            {
                newline = (Newline)token;
                return true;
            }
            newline = null;
            return false;
        }

        public bool TryMatchNewline([MaybeNullWhen(false), NotNullWhen(true)] out Newline? newline)
        {
            if (TryPeekNewline(out newline))
            {
                MatchToken();
                return true;
            }
            return false;
        }

        public Token MatchToken(string? message = null)
        {
            var token = AssertCurrentTokenNotNull(message ?? $"Missing token.");
            NextToken();
            return token;
        }

        public Token PeekToken(string? message = null)
        {
            return AssertCurrentTokenNotNull(message ?? $"Missing token.");
        }

        private readonly IEnumerator<Token> tokenItr;

        public TokenReader(IEnumerable<Token> tokens)
        {
            tokenItr = tokens.GetEnumerator();
            NextToken();
        }
    }
}
