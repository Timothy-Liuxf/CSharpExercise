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
    }
}
