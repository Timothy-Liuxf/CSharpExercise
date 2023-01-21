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
        public SourceLocation Location { get; init; }

        public override string ToString()
        {
            return Type.ToString();
        }

        public Token(TokenType type, SourceLocation location)
        {
            Type = type;
            Location = location;
        }
    }

    internal class StringLiteral : Token
    {
        public string Value { get; init; }

        public override string ToString()
        {
            return '\"' + Value + "\"";
        }

        public StringLiteral(string value, SourceLocation location) : base(TokenType.StringLiteral, location)
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

        public BooleanLiteral(bool value, SourceLocation location) : base(TokenType.BooleanLiteral, location)
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

        public IntegralLiteral(int value, SourceLocation location) : base(TokenType.IntegralLiteral, location)
        {
            Value = value;
        }
    }
}
