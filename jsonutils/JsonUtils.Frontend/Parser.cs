using JsonUtils.Frontend.AST;

namespace JsonUtils.Frontend
{
    internal class Parser
    {
        public ASTNode Parse()
        {
            return this.ast ?? ParseOnce();
        }

        private ASTNode ParseOnce()
        {
            tokens.NextToken();
            var ast = ParseJsonObject();
            var currToken = tokens.CurrentToken;
            if (currToken is not null)
            {
                throw new SyntaxErrorException(currToken.Location, "Extra content.");
            }
            this.ast = ast;
            return ast;
        }

        private JsonObject ParseJsonObject()
        {
            AssertCurrentTokenNotNull("Missing json value.");

            switch (tokens.CurrentToken!.Type)
            {
                case TokenType.LBrace:
                    return ParseClassObject();
                case TokenType.LBracket:
                    return ParseArrayObject();
                case TokenType.NumericLiteral:
                    return ParseNumberValue();
                case TokenType.BooleanLiteral:
                    return ParseBooleanValue();
                case TokenType.StringLiteral:
                    return ParseStringValue();
                case TokenType.NullLiteral:
                    return ParseNullValue();
                default:
                    throw new SyntaxErrorException(tokens.CurrentToken.Location, $"Unexpected token: {tokens.CurrentToken}.");
            }
        }

        private ClassObject ParseClassObject()
        {
            tokens.NextToken(); // Pass token '{'
            var properties = new Dictionary<string, (SourceLocation, JsonObject)>();
            while (true)
            {
                AssertCurrentTokenNotNull("Missing \'}\' token.");

                if (tokens.CurrentToken!.Type == TokenType.RBrace)
                {
                    tokens.NextToken();
                    break;
                }

                MatchToken(TokenType.StringLiteral, $"Expect string literal, found {tokens.CurrentToken!}.");
                var keyToken = (tokens.CurrentToken! as StringLiteral)!;
                string key = keyToken.Value;
                tokens.NextToken();

                MatchToken(TokenType.Colon, $"Expect \':\' token, found {tokens.CurrentToken!}.");
                tokens.NextToken();

                var val = ParseJsonObject();
                if (properties.ContainsKey(key))
                {
                    throw new DuplicatedKeyException(keyToken.Location, key, properties[key].Item1);
                }
                properties.Add(key, (keyToken.Location, val));

                AssertCurrentTokenNotNull("Missing \'}\' token.");
                if (tokens.CurrentToken!.Type == TokenType.Comma)
                {
                    tokens.NextToken();
                }
                else
                {
                    MatchToken(TokenType.RBrace, $"Expect \'}}\' token, found {tokens.CurrentToken!}.");
                    tokens.NextToken();
                    break;
                }
            }
            return new ClassObject(properties);
        }

        private ArrayObject ParseArrayObject()
        {
            tokens.NextToken(); // Pass token '['
            var objects = new List<JsonObject>();
            while (true)
            {
                AssertCurrentTokenNotNull("Missing \']\' token.");

                if (tokens.CurrentToken!.Type == TokenType.RBracket)
                {
                    tokens.NextToken();
                    break;
                }

                var obj = ParseJsonObject();
                objects.Add(obj);

                AssertCurrentTokenNotNull("Missing \']\' token.");
                if (tokens.CurrentToken!.Type == TokenType.Comma)
                {
                    tokens.NextToken();
                }
                else
                {
                    MatchToken(TokenType.RBracket, $"Expect \']\' token, found {tokens.CurrentToken!}.");
                    tokens.NextToken();
                    break;
                }
            }
            return new ArrayObject(objects);
        }

        private NumberValue ParseNumberValue()
        {
            var res = new NumberValue((tokens.CurrentToken as NumericLiteral)!.Value, tokens.CurrentToken!.Location);
            tokens.NextToken();
            return res;
        }

        private BooleanValue ParseBooleanValue()
        {
            var res = new BooleanValue((tokens.CurrentToken as BooleanLiteral)!.Value, tokens.CurrentToken!.Location);
            tokens.NextToken();
            return res;
        }

        private StringValue ParseStringValue()
        {
            var res = new StringValue((tokens.CurrentToken as StringLiteral)!.Value, tokens.CurrentToken!.Location);
            tokens.NextToken();
            return res;
        }

        private NullValue ParseNullValue()
        {
            var res = new NullValue(tokens.CurrentToken!.Location);
            tokens.NextToken();
            return res;
        }

        private void AssertCurrentTokenNotNull(string message)
        {
            if (tokens.CurrentToken is null)
            {
                throw new SyntaxErrorException(endLocation, message);
            }
        }

        private void MatchToken(TokenType token, string message)
        {
            var currToken = tokens.CurrentToken!;
            if (currToken.Type != token)
            {
                throw new SyntaxErrorException(currToken.Location, message);
            }
        }

        private TokenReader tokens;
        private SourceLocation endLocation;
        private ASTNode? ast;

        public Parser(TokenReader tokens, SourceLocation endLocation)
        {
            this.tokens = tokens;
            this.endLocation = endLocation;
        }
    }
}
