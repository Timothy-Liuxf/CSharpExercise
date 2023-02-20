namespace JsonUtils.Utils
{
    public static class JsonTools
    {
        public static (string?, int?) ParseEscapingString(string str)
        {
            var buf = new char[str.Length];
            int pos = 0;
            for (int i = 0; i < str.Length; ++i)
            {
                var ch = str[i];
                if (ch != '\\')
                {
                    buf[pos++] = ch;
                    continue;
                }
                ch = str[++i];

                switch (ch)
                {
                    case 'b':
                        buf[pos++] = '\b';
                        break;
                    case 'f':
                        buf[pos++] = '\f';
                        break;
                    case 'n':
                        buf[pos++] = '\n';
                        break;
                    case 'r':
                        buf[pos++] = '\r';
                        break;
                    case 't':
                        buf[pos++] = '\t';
                        break;
                    case '\"':
                        buf[pos++] = '\"';
                        break;
                    case '/':
                        buf[pos++] = '/';
                        break;
                    case '\\':
                        buf[pos++] = '\\';
                        break;
                    case 'u':
                        if (str.Length - i < 5 || !(str.Substring(i + 1, 4).All(ch =>
                                    char.IsDigit(ch)
                                    || (ch >= 'a' && ch <= 'f')
                                    || (ch >= 'A' && ch <= 'F')
                                )
                            )
                           )
                        {
                            return (null, i);
                        }

                        uint val = 0;
                        for (int j = 0; j < 4; ++j)
                        {
                            ch = str[++i];
                            val = val * 16u + (
                                    char.IsDigit(ch) ? (uint)ch - '0' :
                                    ch >= 'a' && ch <= 'f' ? (uint)ch - 'a' + 10u :
                                    (uint)ch - 'A' + 10u
                                );
                        }
                        buf[pos++] = (char)val;
                        break;
                    default:
                        return (null, i);
                }
            }
            return (new string(buf, 0, pos), null);
        }

        public static string GenerateEscapingString(string str, bool ensureAscii)
        {
            var buf = new ExtendedBuffer<char>(str.Length);
            int pos = 0;
            foreach (var ch in str)
            {
                switch (ch)
                {
                    case '\b':
                        buf[pos++] = '\\';
                        buf[pos++] = 'b';
                        break;
                    case '\f':
                        buf[pos++] = '\\';
                        buf[pos++] = 'f';
                        break;
                    case '\n':
                        buf[pos++] = '\\';
                        buf[pos++] = 'n';
                        break;
                    case '\r':
                        buf[pos++] = '\\';
                        buf[pos++] = 'r';
                        break;
                    case '\t':
                        buf[pos++] = '\\';
                        buf[pos++] = 't';
                        break;
                    case '\"':
                        buf[pos++] = '\\';
                        buf[pos++] = '\"';
                        break;
                    case '/':
                        break;
                    case '\\':
                        buf[pos++] = '\\';
                        buf[pos++] = '\\';
                        break;
                    default:
                        if (
                                char.IsControl(ch)
                                || (ensureAscii && (uint)ch > 127)
                            )
                        {
                            buf[pos++] = '\\';
                            buf[pos++] = 'u';
                            uint val = (uint)ch;
                            for (int i = 0; i < 4; ++i)
                            {
                                var res = val % 16u;
                                val = val / 16u;
                                buf[pos + (3 - i)] = (char)(res < 10u ? res + '0' : res - 10u + 'A');
                            }
                            pos += 4;
                        }
                        else
                        {
                            buf[pos++] = ch;
                        }
                        break;
                }
            }
            return new string(buf.Buffer, 0, pos);
        }
    }
}
