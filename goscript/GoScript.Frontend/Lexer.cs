using GoScript.Utils;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GoScript.Frontend
{
    internal class Lexer
    {
        private bool lexed = false;

        public IEnumerable<Token> Lex()
        {
            if (lexed)
            {
                throw new InternalErrorException("Already lexed.");
            }

            lexed = true;
            var keywords = Keyword.AllKeywords;
            while (this.file.NextLine())
            {
                var currentLine = this.file.CurrentLine!;
                file.NextCharacter();
                while (file.TopCharacter is not null)
                {
                    var ch = file.TopCharacter!.Value;
                    var orgLocation = this.file.Location;
                    var nextChar = true;
                    switch (ch)
                    {
                        case ':':
                            yield return new Punctuator(PunctuatorType.Colon, orgLocation);
                            break;
                        case ',':
                            yield return new Punctuator(PunctuatorType.Comma, orgLocation);
                            break;
                        case ';':
                            yield return new Punctuator(PunctuatorType.Semicolon, orgLocation);
                            break;
                        case '.':
                            if (file.NextCharacter() != null && file.TopCharacter!.Value == '.')
                            {
                                if (file.NextCharacter() != null && file.TopCharacter!.Value == '.')
                                {
                                    yield return new Punctuator(PunctuatorType.Omit, orgLocation);
                                }
                                else
                                {
                                    throw new SyntaxErrorException(orgLocation, "Error token \'{..}\'");
                                }
                            }
                            else
                            {
                                yield return new Punctuator(PunctuatorType.Dot, orgLocation);
                            }
                            nextChar = false;
                            break;
                        case '{':
                            yield return new Punctuator(PunctuatorType.LBrace, orgLocation);
                            break;
                        case '}':
                            yield return new Punctuator(PunctuatorType.RBrace, orgLocation);
                            break;
                        case '[':
                            yield return new Punctuator(PunctuatorType.LBracket, orgLocation);
                            break;
                        case ']':
                            yield return new Punctuator(PunctuatorType.RBracket, orgLocation);
                            break;
                        case '(':
                            yield return new Punctuator(PunctuatorType.LParen, orgLocation);
                            break;
                        case ')':
                            yield return new Punctuator(PunctuatorType.RParan, orgLocation);
                            break;
                        case '+':
                            if (file.NextCharacter() != null && file.TopCharacter!.Value == '+')
                            {
                                yield return new Punctuator(PunctuatorType.Increment, orgLocation);
                            }
                            else
                            {
                                nextChar = false;
                                yield return new Punctuator(PunctuatorType.Add, orgLocation);
                            }
                            break;
                        case '-':
                            if (file.NextCharacter() != null && file.TopCharacter!.Value == '-')
                            {
                                yield return new Punctuator(PunctuatorType.Decrement, orgLocation);
                            }
                            else
                            {
                                nextChar = false;
                                yield return new Punctuator(PunctuatorType.Sub, orgLocation);
                            }
                            break;
                        case '*':
                            yield return new Punctuator(PunctuatorType.Star, orgLocation);
                            break;
                        case '/':
                            yield return new Punctuator(PunctuatorType.Div, orgLocation);
                            break;
                        case '%':
                            yield return new Punctuator(PunctuatorType.Mod, orgLocation);
                            break;
                        case '>':
                        case '<':
                        case '!':
                        case '=':
                        case '&':
                        case '|':
                        case '~':
                            {
                                var (singlePunctuator, doublePunctuators) = ch switch
                                {
                                    '>' => (PunctuatorType.Greater,
                                            new (char, PunctuatorType)[]
                                            {
                                            ('=', PunctuatorType.GreaterEq),
                                            ('>', PunctuatorType.RShift),
                                            }),
                                    '<' => (PunctuatorType.Less,
                                            new (char, PunctuatorType)[]
                                            {
                                            ('=', PunctuatorType.LessEq),
                                            ('<', PunctuatorType.LShift),
                                            }),
                                    '!' => (PunctuatorType.Not,
                                            new (char, PunctuatorType)[] { ('=', PunctuatorType.NotEqual) }),
                                    '=' => (PunctuatorType.Assign,
                                            new (char, PunctuatorType)[] { ('=', PunctuatorType.Equal) }),
                                    '&' => (PunctuatorType.BitAnd,
                                            new (char, PunctuatorType)[]
                                            {
                                            ('&', PunctuatorType.And),
                                                /* ('=', PunctuatorType.BitAndAssign), */
                                            }),
                                    '|' => (PunctuatorType.BitOr,
                                            new (char, PunctuatorType)[]
                                            {
                                            ('|', PunctuatorType.Or),
                                                /* ('=', PunctuatorType.BitOrAssign), */
                                            }),
                                    '~' => (PunctuatorType.BitNot,
                                            new (char, PunctuatorType)[]
                                            {
                                                /* ('=', PunctuatorType.BitNotAssign), */
                                            }),
                                    _ => throw new InternalErrorException("Internal compiler error."),
                                };
                                var type = ParseDoubleCharPunctuator(ch, singlePunctuator, doublePunctuators, orgLocation);
                                yield return new Punctuator(type, orgLocation);
                                nextChar = false;
                            }
                            break;
                        case '\"':
                            {
                                for (var curChn = file.NextCharacter(); curChn != null; curChn = file.NextCharacter())
                                {
                                    var curCh = curChn.Value;
                                    if (curCh == '\\')
                                    {
                                        if (file.NextCharacter() == null)
                                        {
                                            throw new NotImplementedException($"At {orgLocation}: Multiline strings have not been supported yet.");
                                        }
                                    }
                                    else if (curCh == ch)
                                    {
                                        break;
                                    }
                                }

                                var rawValue = currentLine.Substring(orgLocation.Column, file.Location.Column - orgLocation.Column - 1);
                                var (value, errPos) = Escaping.ParseEscapingString(rawValue);
                                if (value == null)
                                {
                                    var errCh = rawValue[errPos!.Value];
                                    var errLen = Math.Min(5, rawValue.Length - errPos.Value);
                                    var errStr = errCh != 'u' ? errCh.ToString() : rawValue.Substring(errPos.Value, errLen);
                                    throw new SyntaxErrorException(new SourceLocation() { Line = orgLocation.Line, Column = orgLocation.Column + errPos.Value }, $"Error escaping character: \\{errStr}.");
                                }
                                yield return new StringLiteral(value, orgLocation);
                            }
                            break;
                        default:
                            if (ch == '_' || char.IsLetter(ch))
                            {
                                // Keyword or identifier
                                bool valid = true;
                                for (var curChn = file.NextCharacter(); curChn is not null; curChn = file.NextCharacter())
                                {
                                    var curCh = curChn.Value;
                                    if (char.IsWhiteSpace(curCh) || Punctuator.IsPunctuator(curCh.ToString()))
                                    {
                                        break;
                                    }
                                    else if (!char.IsLetterOrDigit(curCh) && curCh != '_')
                                    {
                                        valid = false;
                                    }
                                }

                                int matchLen = file.Location.Column - orgLocation.Column;
                                string matchStr = currentLine.Substring(orgLocation.Column - 1, matchLen);
                                if (valid)
                                {
                                    if (keywords.ContainsKey(matchStr))
                                    {
                                        yield return matchStr switch
                                        {
                                            "true" => new BoolLiteral(true, orgLocation),
                                            "false" => new BoolLiteral(false, orgLocation),
                                            _ => new Keyword(keywords[matchStr], orgLocation),
                                        };
                                    }
                                    else
                                    {
                                        yield return new Identifier(matchStr, orgLocation);
                                    }
                                }
                                else
                                {
                                    throw new SyntaxErrorException(orgLocation,
                                        $"Invalid identifier: {matchStr}.");
                                }
                                nextChar = false;
                            }
                            else if (!char.IsWhiteSpace(ch))
                            {
                                var hexRegex = new Regex(@"\G0[Xx][0-9A-Fa-f]+");
                                var scienceRegex = new Regex(@"\G[1-9](\.[0-9]+)?[Ee][+-]?[0-9]+");
                                var floatRegex = new Regex(@"\G(([1-9][0-9]*|0)?\.[0-9]+)");
                                var integerRegex = new Regex(@"\G([1-9][0-9]*|0)");

                                var MatchRegex = (Regex rgx, string input, int startat, out Match match) =>
                                {
                                    match = rgx.Match(input, startat);
                                    return match.Success;
                                };

                                if (MatchRegex(hexRegex, currentLine, orgLocation.Column - 1, out var match))
                                {
                                    var value = match.Value.Substring(2);
                                    for (int i = 0; i < value.Length + 2; ++i)
                                    {
                                        file.NextCharacter();
                                    }
                                    if (file.TopCharacter != null)
                                    {
                                        var nextCh = file.TopCharacter.Value;
                                        if (!(char.IsWhiteSpace(nextCh)
                                            || (Punctuator.IsPunctuator(nextCh.ToString()) && nextCh != '.')))
                                        {
                                            throw new SyntaxErrorException(orgLocation, $"Error number literal at {orgLocation}.");

                                        }
                                    }
                                    yield return new IntegerLiteral(ulong.Parse(value, NumberStyles.HexNumber), orgLocation);
                                }
                                else if (MatchRegex(scienceRegex, currentLine, orgLocation.Column - 1, out match))
                                {
                                    var value = match.Value;
                                    for (int i = 0; i < value.Length; ++i)
                                    {
                                        file.NextCharacter();
                                    }
                                    if (file.TopCharacter != null)
                                    {
                                        var nextCh = file.TopCharacter.Value;
                                        if (!(char.IsWhiteSpace(nextCh)
                                            || (Punctuator.IsPunctuator(nextCh.ToString()) && nextCh != '.')))
                                        {
                                            throw new SyntaxErrorException(orgLocation, $"Error number literal at {orgLocation}.");
                                        }
                                    }
                                    yield return new FloatLiteral(double.Parse(value, NumberStyles.Float), orgLocation);
                                }
                                else if (MatchRegex(floatRegex, currentLine, orgLocation.Column - 1, out match))
                                {
                                    var value = match.Value;
                                    for (int i = 0; i < value.Length; ++i)
                                    {
                                        file.NextCharacter();
                                    }
                                    if (file.TopCharacter != null)
                                    {
                                        var nextCh = file.TopCharacter.Value;
                                        if (!(char.IsWhiteSpace(nextCh)
                                            || (Punctuator.IsPunctuator(nextCh.ToString()) && nextCh != '.')))
                                        {
                                            throw new SyntaxErrorException(orgLocation, $"Error number literal at {orgLocation}.");
                                        }
                                    }
                                    yield return new FloatLiteral(double.Parse(value), orgLocation);
                                }
                                else if (MatchRegex(integerRegex, currentLine, orgLocation.Column - 1, out match))
                                {
                                    var value = match.Value;
                                    for (int i = 0; i < value.Length; ++i)
                                    {
                                        file.NextCharacter();
                                    }
                                    if (file.TopCharacter != null)
                                    {
                                        var nextCh = file.TopCharacter.Value;
                                        if (!(char.IsWhiteSpace(nextCh)
                                            || (Punctuator.IsPunctuator(nextCh.ToString()) && nextCh != '.')))
                                        {
                                            throw new SyntaxErrorException(orgLocation, $"Error number literal at {orgLocation}.");
                                        }
                                    }
                                    yield return new IntegerLiteral(ulong.Parse(value), orgLocation);
                                }
                                else
                                {
                                    throw new SyntaxErrorException(orgLocation, $"Error token: {ch}");
                                }
                                nextChar = false;
                            }
                            break;
                    }
                    if (nextChar)
                    {
                        file.NextCharacter();
                    }
                }
                yield return new Newline(file.Location);
            }
        }

        private PunctuatorType ParseDoubleCharPunctuator(char firstChar, PunctuatorType singlePunctrator, (char, PunctuatorType)[] doublePunctrators, SourceLocation orgLocation)
        {
            if (file.NextCharacter() != null)
            {
                foreach ((var followChar, var doublePunctrator) in doublePunctrators)
                {
                    if (file.TopCharacter!.Value == followChar)
                    {
                        if (file.NextCharacter() != null && file.TopCharacter!.Value == followChar)
                        {
                            int followCnt = 1;
                            while (file.NextCharacter() != null && file.TopCharacter!.Value == followChar)
                            {
                                ++followCnt;
                            }
                            throw new SyntaxErrorException(orgLocation, $"Error token \'{firstChar}{new string(followChar, followCnt)}\'");
                        }
                        else
                        {
                            return doublePunctrator;
                        }
                    }
                }
            }
            return singlePunctrator;
        }

        private SourceFile file;

        public Lexer(SourceFile file)
        {
            this.file = file;
        }
    }
}
