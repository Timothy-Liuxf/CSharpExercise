using System.Linq;

namespace JsonUtils.Frontend
{
    internal class Lexer
    {
        public IList<Token> Lex(SourceManager source)
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
                        default:
                            {
                                if (char.IsDigit(ch))
                                {
                                    var startLoc = source.Location.Column;
                                    while (source.NextCharacter() && char.IsDigit(source.TopCharacter!.Value)) { }
                                    var endLoc = source.Location.Column;
                                    if (ch == '0')
                                    {
                                        if (startLoc + 1 == endLoc)
                                        {
                                            tokens.Add(new IntegralLiteral(0, orgLocation));
                                        }
                                        else
                                        {
                                            throw new SyntaxErrorException(new SourceLocation(source.Location.Line, startLoc), "Error number!");
                                        }
                                    }
                                    else
                                    {
                                        int value = 0;
                                        try
                                        {
                                            checked
                                            {
                                                for (int i = startLoc - 1; i < endLoc - 1; ++i)
                                                {
                                                    value = value * 10 + (currentLine[i] - '0');
                                                }
                                            }
                                            tokens.Add(new IntegralLiteral(value, orgLocation));
                                        }
                                        catch (OverflowException)
                                        {
                                            throw new IntegerOverflowException(new SourceLocation(source.Location.Line, startLoc), source.CurrentLine!.Substring(startLoc - 1, endLoc - startLoc));
                                        }
                                    }
                                    nextChar = false;
                                }
                                else if (!char.IsWhiteSpace(ch))
                                {
                                    throw new SyntaxErrorException(source.Location, "Error token.");
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
            return tokens;
        }

        public SourceLocation EndLocation { get; private set; }
    }
}
