namespace JsonUtils.Frontend
{
    public enum TokenType
    {
        None,
        LBrace,             // {
        RBrace,             // }
        LBracket,           // [
        RBracket,           // ]
        Comma,              // ,
        Colon,              // :
        StringLiteral,      // "string"
        BooleanLiteral,     // Bool
        IntegralLiteral,    // int
    }

    public class Token
    {
        public const string TrueLiteral = "true";
        public const string FalseLiteral = "false";

        public TokenType Type { get; set; }

        public override string ToString()
        {
            return Type.ToString();
        }

        public Token(TokenType type)
        {
            Type = type;
        }
    }

    internal class StringLiteral : Token
    {
        public string Value { get; init; }

        public override string ToString()
        {
            return '\"' + Value + "\"";
        }

        public StringLiteral(string value) : base(TokenType.StringLiteral)
        {
            Value = value;
        }
    }

    internal class BooleanLiteral : Token
    {
        public bool Value { get; init; }

        public override string ToString()
        {
            return Value ? "true" : "false";
        }

        public BooleanLiteral(bool value) : base(TokenType.BooleanLiteral)
        {
            Value = value;
        }
    }

    internal class IntegralLiteral : Token
    {
        public int Value { get; init; }

        public override string ToString()
        {
            return Value.ToString();
        }

        public IntegralLiteral(int value) : base(TokenType.IntegralLiteral)
        {
            Value = value;
        }
    }
}
