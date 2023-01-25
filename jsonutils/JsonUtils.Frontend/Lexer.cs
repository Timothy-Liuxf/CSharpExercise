using System.Text.RegularExpressions;

namespace JsonUtils.Frontend
{
    internal class Lexer
    {
        public IList<Token> Lex()
        {
            return this.tokens ?? LexOnce();
        }

        private IList<Token> LexOnce()
        {
            var tokens = new List<Token>();
            var totalEndLocation = new SourceLocation(0, 0);
            while (source.NextLine())
            {
                var currentLine = source.CurrentLine!;
                totalEndLocation.Line = source.Location.Line;
                totalEndLocation.Column = 0;
                source.NextCharacter();
                for (var chn = source.TopCharacter; chn is not null; chn = source.TopCharacter)
                {
                    bool nextChar = true;
                    var ch = chn.Value;
                    var orgLocation = source.Location;
                    totalEndLocation.Column = orgLocation.Column;
                    switch (ch)
                    {
                        case '{':
                            tokens.Add(new Token(TokenType.LBrace, orgLocation));
                            break;
                        case '}':
                            tokens.Add(new Token(TokenType.RBrace, orgLocation));
                            break;
                        case '[':
                            tokens.Add(new Token(TokenType.LBracket, orgLocation));
                            break;
                        case ']':
                            tokens.Add(new Token(TokenType.RBracket, orgLocation));
                            break;
                        case ',':
                            tokens.Add(new Token(TokenType.Comma, orgLocation));
                            break;
                        case ':':
                            tokens.Add(new Token(TokenType.Colon, orgLocation));
                            break;
                        case '"':
                            {
                                var startLoc = source.Location.Column;
                                var endLoc = -1;
                                while (source.NextCharacter())
                                {
                                    if (source.TopCharacter! == '"')
                                    {
                                        endLoc = source.Location.Column;
                                        break;
                                    }
                                }
                                if (endLoc == -1)
                                {
                                    throw new SyntaxErrorException(source.Location, "Missing '\"'.");
                                }
                                tokens.Add(new StringLiteral(currentLine.Substring(startLoc, endLoc - startLoc - 1), orgLocation));
                            }
                            break;
                        case 't':
                            {
                                var startLoc = source.Location.Column;
                                if (string.CompareOrdinal(currentLine, startLoc - 1, Token.TrueLiteral, 0, Token.TrueLiteral.Length) != 0)
                                {
                                    throw new SyntaxErrorException(new SourceLocation(source.Location.Line, startLoc), "Error token.");
                                }
                                for (int i = 0; i < Token.TrueLiteral.Length - 1; ++i)
                                {
                                    source.NextCharacter();
                                }
                                tokens.Add(new BooleanLiteral(true, orgLocation));
                            }
                            break;
                        case 'f':
                            {
                                var startLoc = source.Location.Column;
                                if (string.CompareOrdinal(currentLine, startLoc - 1, Token.FalseLiteral, 0, Token.FalseLiteral.Length) != 0)
                                {
                                    throw new SyntaxErrorException(new SourceLocation(source.Location.Line, startLoc), "Error token.");
                                }
                                for (int i = 0; i < Token.FalseLiteral.Length - 1; ++i)
                                {
                                    source.NextCharacter();
                                }
                                tokens.Add(new BooleanLiteral(false, orgLocation));
                            }
                            break;
                        case 'n':
                            {
                                var startLoc = source.Location.Column;
                                if (string.CompareOrdinal(currentLine, startLoc - 1, Token.NullLiteral, 0, Token.NullLiteral.Length) != 0)
                                {
                                    throw new SyntaxErrorException(new SourceLocation(source.Location.Line, startLoc), "Error token.");
                                }
                                for (int i = 0; i < Token.NullLiteral.Length - 1; ++i)
                                {
                                    source.NextCharacter();
                                }
                                tokens.Add(new Token(TokenType.NullLiteral, orgLocation));
                            }
                            break;
                        default:
                            {
                                if (!char.IsWhiteSpace(ch))
                                {
                                    if (MatchNumbers(out var value))
                                    {
                                        nextChar = false;
                                        tokens.Add(new NumericLiteral(value, orgLocation));
                                    }
                                    else
                                    {
                                        throw new SyntaxErrorException(source.Location, "Error token.");
                                    }
                                }
                            }
                            break;
                    }

                    if (nextChar)
                    {
                        source.NextCharacter();
                    }
                }
            }
            EndLocation = totalEndLocation;
            this.tokens = tokens;
            return tokens;
        }

        private bool MatchNumbers(out string value)
        {
            var orgLocation = source.Location!;
            var numberRegex = new Regex(@"[-+]?(([1-9][0-9]*|0)?\.[0-9]+|([1-9][0-9]*|0))");
            var scienceRegex = new Regex(@"[-+]?[1-9](\.[0-9]+)?[Ee][+-]?[0-9]+");
            var hexRegex = new Regex(@"0[Xx][0-9A-Fa-f]+");     // Heximal numbers support in JSON5
            var regex = new Regex($"\\G({hexRegex}|{scienceRegex}|{numberRegex})");

            // Match numbers
            var match = regex.Match(source.CurrentLine!, orgLocation.Column - 1);
            if (match.Success)
            {
                value = match.Value;
                for (int i = 0; i < value.Length; ++i)
                {
                    source.NextCharacter();
                }
                if (source.TopCharacter is not null
                    && (char.IsDigit(source.TopCharacter.Value) || source.TopCharacter.Value == '.'))
                {
                    // 00...0...
                    throw new SyntaxErrorException(orgLocation, "Error token.");
                }
                return true;
            }

            value = "";
            return false;
        }

        public SourceLocation EndLocation { get; private set; }
        private IList<Token>? tokens;
        private SourceManager source;

        public Lexer(SourceManager source)
        {
            this.source = source;
        }
    }
}
