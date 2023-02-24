using GoScript.Utils;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

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
                            if (file.NextCharacter() && file.TopCharacter!.Value == '.')
                            {
                                if (file.NextCharacter() && file.TopCharacter!.Value == '.')
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
                            if (file.NextCharacter() && file.TopCharacter!.Value == '+')
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
                            if (file.NextCharacter() && file.TopCharacter!.Value == '-')
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
                            break;
                        default:
                            if (!char.IsWhiteSpace(ch))
                            {
                                throw new SyntaxErrorException(orgLocation, $"Error token: {ch}");
                            }
                            break;
                    }
                    if (nextChar)
                    {
                        file.NextCharacter();
                    }
                }
            }
        }

        private PunctuatorType ParseDoubleCharPunctuator(char firstChar, PunctuatorType singlePunctrator, (char, PunctuatorType)[] doublePunctrators, SourceLocation orgLocation)
        {
            if (file.NextCharacter())
            {
                foreach ((var followChar, var doublePunctrator) in doublePunctrators)
                {
                    if (file.TopCharacter!.Value == followChar)
                    {
                        if (file.NextCharacter() && file.TopCharacter!.Value == followChar)
                        {
                            int followCnt = 1;
                            while (file.NextCharacter() && file.TopCharacter!.Value == followChar)
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
