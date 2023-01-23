using JsonUtils.Frontend;
using JsonUtils.Frontend.AST;

namespace JsonUtils.Formatter
{
    internal class FormatHelper : IVisitor
    {
        public void Visit(ClassObject obj)
        {
            if (obj.Properties.Count() == 0)
            {
                obj.StringAttribute = new string[] { "{}" };
                return;
            }

            var strAttr = new List<string>();
            strAttr.Add("{");
            foreach (var (key, (_, val)) in obj.Properties)
            {
                var keyStr = DumpString(key);
                val.Accept(this);
                var valStr = val.StringAttribute!;
                var cnt = valStr.Length;
                if (cnt == 0)
                {
                    strAttr.Add(Indentation + keyStr + ": ,");
                }
                else if (cnt == 1)
                {
                    strAttr.Add(Indentation + keyStr + ": " + valStr[0] + ",");
                }
                else
                {
                    strAttr.Add(Indentation + keyStr + ": " + valStr[0]);
                    for (int i = 1; i < cnt - 1; ++i)
                    {
                        strAttr.Add(Indentation + valStr[i]);
                    }
                    strAttr.Add(Indentation + valStr[cnt - 1] + ",");
                }
            }
            strAttr.Add("}");
            obj.StringAttribute = strAttr.ToArray();
        }

        public void Visit(ArrayObject array)
        {
            if (array.Objects.Count() == 0)
            {
                array.StringAttribute = new string[] { "[]" };
                return;
            }

            var strAttr = new List<string>();
            strAttr.Add("[");
            foreach (var elem in array.Objects)
            {
                elem.Accept(this);
                var elemStr = elem.StringAttribute!;
                var cnt = elemStr.Length;
                if (cnt == 0)
                {
                    strAttr.Add(Indentation + ",");
                }
                else
                {
                    for (int i = 0; i < cnt - 1; ++i)
                    {
                        strAttr.Add(Indentation + elemStr[i]);
                    }
                    strAttr.Add(Indentation + elemStr[cnt - 1] + ",");
                }
            }
            strAttr.Add("]");
            array.StringAttribute = strAttr.ToArray();
        }

        public void Visit(StringValue stringValue)
        {
            stringValue.StringAttribute = new[]
            {
                DumpString(stringValue.Value),
            };
        }

        public void Visit(NumberValue numberValue)
        {
            numberValue.StringAttribute = new[]
            {
                numberValue.Value.ToString(),
            };
        }

        public void Visit(BooleanValue booleanValue)
        {
            booleanValue.StringAttribute = new[]
            {
                booleanValue.Value ? Token.TrueLiteral : Token.FalseLiteral,
            };
        }

        private string DumpString(string str)
        {
            return "\"" + str + "\"";
        }

        public string Indentation { get; init; } = "    ";
    }
}
