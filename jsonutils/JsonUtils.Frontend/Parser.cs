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
            var ast = ParseJsonObject();
            tokens.AssertNullToken("Extra content.");
            this.ast = ast;
            return ast;
        }

        private JsonObject ParseJsonObject()
        {
            var currToken = tokens.AssertCurrentTokenNotNull("Missing json value.");

            switch (currToken.Type)
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
                    throw new SyntaxErrorException(currToken.Location, $"Unexpected token: {currToken}.");
            }
        }

        private ClassObject ParseClassObject()
        {
            var orgLocation = tokens.CurrentToken!.Location;
            tokens.MatchToken(false); // Pass token '{'
            var properties = new Dictionary<string, (SourceLocation, JsonObject)>();
            while (true)
            {
                var currToken = tokens.AssertCurrentTokenNotNull("Missing \'}\' token.");

                if (currToken.Type == TokenType.RBrace)
                {
                    tokens.MatchToken(false);
                    break;
                }

                var keyToken = (StringLiteral)tokens.MatchToken(TokenType.StringLiteral, "string literal");
                string key = keyToken.Value;

                tokens.MatchToken(TokenType.Colon, "\':\'");

                var val = ParseJsonObject();
                if (properties.ContainsKey(key))
                {
                    throw new DuplicatedKeyException(keyToken.Location, key, properties[key].Item1);
                }
                properties.Add(key, (keyToken.Location, val));

                currToken = tokens.AssertCurrentTokenNotNull("Missing \'}\' token.");
                if (currToken.Type == TokenType.Comma)
                {
                    tokens.MatchToken(false);
                }
                else
                {
                    tokens.MatchToken(TokenType.RBrace, "\'}\'");
                    break;
                }
            }
            return new ClassObject(properties, orgLocation);
        }

        private ArrayObject ParseArrayObject()
        {
            var orgLocation = tokens.CurrentToken!.Location;
            tokens.MatchToken(false); // Pass token '['
            var objects = new List<JsonObject>();
            while (true)
            {
                var currToken = tokens.AssertCurrentTokenNotNull("Missing \']\' token.");

                if (currToken.Type == TokenType.RBracket)
                {
                    tokens.MatchToken(false);
                    break;
                }

                var obj = ParseJsonObject();
                objects.Add(obj);

                currToken = tokens.AssertCurrentTokenNotNull("Missing \']\' token.");
                if (currToken.Type == TokenType.Comma)
                {
                    tokens.MatchToken(false);
                }
                else
                {
                    tokens.MatchToken(TokenType.RBracket, "\']\'");
                    break;
                }
            }
            return new ArrayObject(objects, orgLocation);
        }

        private NumberValue ParseNumberValue()
        {
            var res = new NumberValue((tokens.CurrentToken as NumericLiteral)!.Value, tokens.CurrentToken!.Location);
            tokens.MatchToken(false);
            return res;
        }

        private BooleanValue ParseBooleanValue()
        {
            var res = new BooleanValue((tokens.CurrentToken as BooleanLiteral)!.Value, tokens.CurrentToken!.Location);
            tokens.MatchToken(false);
            return res;
        }

        private StringValue ParseStringValue()
        {
            var res = new StringValue((tokens.CurrentToken as StringLiteral)!.Value, tokens.CurrentToken!.Location);
            tokens.MatchToken(false);
            return res;
        }

        private NullValue ParseNullValue()
        {
            var res = new NullValue(tokens.CurrentToken!.Location);
            tokens.MatchToken(false);
            return res;
        }

        private TokenReader tokens;
        private ASTNode? ast;

        public Parser(TokenReader tokens)
        {
            this.tokens = tokens;
        }
    }
}
