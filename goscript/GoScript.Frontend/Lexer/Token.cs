using GoScript.Utils;

namespace GoScript.Frontend.Lexer
{
    public enum TokenType
    {
        None,
        Punctuator,
        Keyword,
        Literal,
        Identifier,
        Newline,
    }

    public abstract class Token
    {
        public abstract TokenType TokenCatagory { get; }

        public SourceLocation Location { get; private init; }

        public Token(SourceLocation location)
        {
            Location = location;
        }
    }

    public enum PunctuatorType
    {
        None,
        Colon,      // :
        Comma,      // ,
        Semicolon,  // ;
        Dot,        // .
        LBrace,     // {
        RBrace,     // }
        LBracket,   // [
        RBracket,   // ]
        LParen,     // (
        RParan,     // )
        Omit,       // ...
        Add,        // +
        Sub,        // -
        Star,       // *
        Div,        // /
        Mod,        // %
        Increment,  // ++
        Decrement,  // --
        Equal,      // ==
        NotEqual,   // !=
        Greater,    // >
        Less,       // <
        GreaterEq,  // >=
        LessEq,     // <=
        And,        // &&
        Or,         // ||
        Not,        // !
        BitAnd,     // &
        BitOr,      // |
        BitNot,     // ~
        BitXor,     // ^
        LShift,     // <<
        RShift,     // >>
        Assign,     // =
    }

    public class Punctuator : Token
    {
        public override TokenType TokenCatagory => TokenType.Punctuator;

        public PunctuatorType Type { get; private init; }

        public static string? GetPunctuator(PunctuatorType type)
        {
            return type switch
            {
                PunctuatorType.None => null,
                PunctuatorType.Colon => ":",
                PunctuatorType.Comma => ",",
                PunctuatorType.Semicolon => ";",
                PunctuatorType.Dot => ".",
                PunctuatorType.LBrace => "{",
                PunctuatorType.RBrace => "}",
                PunctuatorType.LBracket => "[",
                PunctuatorType.RBracket => "]",
                PunctuatorType.LParen => "(",
                PunctuatorType.RParan => ")",
                PunctuatorType.Omit => "...",
                PunctuatorType.Add => "+",
                PunctuatorType.Sub => "-",
                PunctuatorType.Star => "*",
                PunctuatorType.Div => "/",
                PunctuatorType.Mod => "%",
                PunctuatorType.Increment => "++",
                PunctuatorType.Decrement => "--",
                PunctuatorType.Equal => "==",
                PunctuatorType.NotEqual => "!=",
                PunctuatorType.Greater => ">",
                PunctuatorType.Less => "<",
                PunctuatorType.GreaterEq => ">=",
                PunctuatorType.LessEq => "<=",
                PunctuatorType.And => "&&",
                PunctuatorType.Or => "||",
                PunctuatorType.Not => "!",
                PunctuatorType.BitAnd => "&",
                PunctuatorType.BitOr => "|",
                PunctuatorType.BitNot => "~",
                PunctuatorType.BitXor => "^",
                PunctuatorType.LShift => "<<",
                PunctuatorType.RShift => ">>",
                PunctuatorType.Assign => "=",
                _ => null,
            };
        }

        private static IEnumerable<string> GetAllPunctuatorsImpl()
        {
            foreach (var val in Enum.GetValues(typeof(PunctuatorType)).Cast<PunctuatorType>())
            {
                var punctuator = GetPunctuator(val);
                if (punctuator is not null)
                {
                    yield return punctuator;
                }
            }
        }

        private static IReadOnlySet<string>? allPunctuators;
        public static IEnumerable<string> AllPunctuators
            => allPunctuators ?? (allPunctuators = GetAllPunctuatorsImpl().ToHashSet());

        public static bool IsPunctuator(string str)
        {
            return AllPunctuators.Contains(str);
        }

        public override string ToString()
        {
            return Type == PunctuatorType.None ? "None" : GetPunctuator(Type)
                ?? throw new InternalErrorException($"Bad punctuator token {Type} at {Location}.");
        }

        public Punctuator(PunctuatorType type, SourceLocation location) : base(location)
        {
            Type = type;
        }
    }

    public enum KeywordType
    {
        None,
        Package,    // package
        Import,     // import
        Func,       // func
        Var,        // var
        If,         // if
        For,        // for
        Break,      // break
        Continue,   // continue
        Return,     // return
        Type,       // type
        Struct,     // struct
        Interface,  // interface
        Map,        // map
        Int8,       // int8
        UInt8,      // uint8
        Int16,      // int16
        UInt16,     // uint16
        Int32,      // int16
        UInt32,     // uint16
        Int64,      // int64
        UInt64,     // uint64
        Float32,    // float32
        Float64,    // float64
        Bool,       // bool
        True,       // true
        False,      // false
        Nil,        // nil
    }

    public class Keyword : Token
    {
        public override TokenType TokenCatagory => TokenType.Keyword;

        public KeywordType Type { get; private init; }

        public bool IsTypeKeyword()
        {
            return Type switch
            {
                KeywordType.Int8 or
                KeywordType.UInt8 or
                KeywordType.Int16 or
                KeywordType.UInt16 or
                KeywordType.Int32 or
                KeywordType.UInt32 or
                KeywordType.Int64 or
                KeywordType.UInt64 or
                KeywordType.Float32 or
                KeywordType.Float64 => true,
                _ => false,
            };
        }

        public static string? GetKeywordString(KeywordType type)
        {
            return type switch
            {
                KeywordType.None => null,
                KeywordType.Package => "package",
                KeywordType.Import => "import",
                KeywordType.Func => "func",
                KeywordType.Var => "var",
                KeywordType.If => "if",
                KeywordType.For => "for",
                KeywordType.Break => "break",
                KeywordType.Continue => "continue",
                KeywordType.Return => "return",
                KeywordType.Type => "type",
                KeywordType.Struct => "struct",
                KeywordType.Interface => "interface",
                KeywordType.Map => "map",
                KeywordType.Int8 => "int8",
                KeywordType.UInt8 => "uint8",
                KeywordType.Int16 => "int16",
                KeywordType.UInt16 => "uint16",
                KeywordType.Int32 => "int32",
                KeywordType.UInt32 => "uint32",
                KeywordType.Int64 => "int64",
                KeywordType.UInt64 => "uint64",
                KeywordType.Float32 => "float32",
                KeywordType.Float64 => "float64",
                KeywordType.Bool => "bool",
                KeywordType.True => "true",
                KeywordType.False => "false",
                KeywordType.Nil => "nil",
                _ => null,
            };
        }

        private static IEnumerable<KeywordType> GetAllKeywordsImpl()
        {
            foreach (var val in Enum.GetValues(typeof(KeywordType)).Cast<KeywordType>())
            {
                if (val != KeywordType.None)
                {
                    yield return val;
                }
            }
        }

        private static IReadOnlyDictionary<string, KeywordType>? allKeywords;
        public static IReadOnlyDictionary<string, KeywordType> AllKeywords
            => allKeywords ?? (allKeywords = GetAllKeywordsImpl().ToDictionary(type => GetKeywordString(type)
                    ?? throw new InternalErrorException($"Keyword type: {type} does not have its string.")));

        public override string ToString()
        {
            return Type == KeywordType.None ? "None" : GetKeywordString(Type) ?? throw new InternalErrorException($"Bad keyword token {Type} at {Location}.");
        }

        public Keyword(KeywordType type, SourceLocation location) : base(location)
        {
            Type = type;
        }
    }

    public enum LiteralType
    {
        None,
        IntegerLiteral,
        FloatLiteral,
        BoolLiteral,
        StringLiteral,
    }

    public abstract class Literal : Token
    {
        public override TokenType TokenCatagory => TokenType.Literal;

        public abstract LiteralType Type { get; }

        public override string ToString()
        {
            return GetType().GetProperty("Value")!.GetValue(this)!.ToString()
                ?? throw new InternalErrorException($"Fail to get the string of token {GetType().Name}.");
        }

        public Literal(SourceLocation location) : base(location) { }
    }

    public class IntegerLiteral : Literal
    {
        public override LiteralType Type => LiteralType.IntegerLiteral;

        public ulong Value { get; private init; }

        public IntegerLiteral(ulong value, SourceLocation location) : base(location)
        {
            Value = value;
        }
    }

    public class FloatLiteral : Literal
    {
        public override LiteralType Type => LiteralType.FloatLiteral;

        public double Value { get; private init; }

        public FloatLiteral(double value, SourceLocation location) : base(location)
        {
            Value = value;
        }
    }

    public class StringLiteral : Literal
    {
        public override LiteralType Type => LiteralType.FloatLiteral;

        public string Value { get; private init; }

        public override string ToString()
        {
            return "\"" + Escaping.GenerateEscapingString(Value, false) + "\"";
        }

        public StringLiteral(string value, SourceLocation location) : base(location)
        {
            Value = value;
        }
    }

    public class BoolLiteral : Literal
    {
        public override LiteralType Type => LiteralType.FloatLiteral;

        public bool Value { get; private init; }

        public BoolLiteral(bool value, SourceLocation location) : base(location)
        {
            Value = value;
        }
    }

    public class Identifier : Token
    {
        public override TokenType TokenCatagory => TokenType.Identifier;

        public string Name { get; private init; }

        public override string ToString()
        {
            return $"<Id:{Name}>";
        }

        public Identifier(string name, SourceLocation location) : base(location)
        {
            Name = name;
        }
    }

    public class Newline : Token
    {
        public override TokenType TokenCatagory => TokenType.Newline;

        public override string ToString()
        {
            return "Newline\n";
        }

        public Newline(SourceLocation location) : base(location) { }
    }
}
