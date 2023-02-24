using GoScript.Utils;
using System;
using System.Diagnostics;

namespace GoScript.Frontend
{
    public enum TokenType
    {
        None,
        Punctuator,
        Keyword,
        Literal,
        Identifier,
    }

    public abstract class Token
    {
        public abstract TokenType TokenCatagory { get; }

        public SourceLocation Location { get; private init; }

        public Token(SourceLocation location)
        {
            this.Location = location;
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
        LShift,     // <<
        RShift,     // >>
        Assign,     // =
    }

    public class Punctuator : Token
    {
        public override TokenType TokenCatagory => TokenType.Punctuator;

        public PunctuatorType Type;

        public override string ToString()
        {
            return this.Type switch
            {
                PunctuatorType.None => "None",
                PunctuatorType.Colon => ":",
                PunctuatorType.Comma => ",",
                PunctuatorType.Semicolon => ";",
                PunctuatorType.Dot => ".",
                PunctuatorType.LBrace => "{",
                PunctuatorType.RBrace => "}",
                PunctuatorType.LBracket => "[",
                PunctuatorType.RBracket => "]",
                PunctuatorType.LParen => "",
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
                PunctuatorType.LShift => "<<",
                PunctuatorType.RShift => ">>",
                PunctuatorType.Assign => "=",
                _ => throw new InternalErrorException($"Bad punctuator token {this.Type} at {this.Location}.")
            };
        }

        public Punctuator(PunctuatorType type, SourceLocation location) : base(location)
        {
            this.Type = type;
        }
    }

    public enum KeywordType
    {
        None,
        Package,    // package
        Import,     // import
        Func,       // func
        Var,        // var
        If,         // If
        For,        // for
        Break,      // break
        Continue,   // continue
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
    }

    public class Keyword : Token
    {
        public override TokenType TokenCatagory => TokenType.Keyword;

        public KeywordType Type;

        public override string ToString()
        {
            return this.Type switch
            {
                KeywordType.None => "None",
                KeywordType.Package => "package",
                KeywordType.Import => "import",
                KeywordType.Func => "func",
                KeywordType.Var => "var",
                KeywordType.If => "If",
                KeywordType.For => "for",
                KeywordType.Break => "break",
                KeywordType.Continue => "continue",
                KeywordType.Type => "type",
                KeywordType.Struct => "struct",
                KeywordType.Interface => "interface",
                KeywordType.Map => "map",
                KeywordType.Int8 => "int8",
                KeywordType.UInt8 => "uint8",
                KeywordType.Int16 => "int16",
                KeywordType.UInt16 => "uint16",
                KeywordType.Int32 => "int16",
                KeywordType.UInt32 => "uint16",
                KeywordType.Int64 => "int64",
                KeywordType.UInt64 => "uint64",
                KeywordType.Float32 => "float32",
                KeywordType.Float64 => "float64",
                KeywordType.Bool => "bool",
                KeywordType.True => "true",
                KeywordType.False => "false",
                _ => throw new InternalErrorException($"Bad keyword token {this.Type} at {this.Location}.")
            };
        }

        public Keyword(KeywordType type, SourceLocation location) : base(location)
        {
            this.Type = type;
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
            return this.GetType().GetProperty("Value")!.GetValue(this)!.ToString()
                ?? throw new InternalErrorException($"Fail to get the string of token {this.GetType().Name}.");
        }

        public Literal(SourceLocation location) : base(location) { }
    }

    public class IntegerLiteral : Literal
    {
        public override LiteralType Type => LiteralType.IntegerLiteral;

        public ulong Value { get; private init; }

        public IntegerLiteral(ulong value, SourceLocation location) : base(location)
        {
            this.Value = value;
        }
    }

    public class FloatLiteral : Literal
    {
        public override LiteralType Type => LiteralType.FloatLiteral;

        public double Value { get; private init; }

        public FloatLiteral(double value, SourceLocation location) : base(location)
        {
            this.Value = value;
        }
    }

    public class StringLiteral : Literal
    {
        public override LiteralType Type => LiteralType.FloatLiteral;

        public string Value { get; private init; }

        public override string ToString()
        {
            return "\"" + Value + "\"";
        }

        public StringLiteral(string value, SourceLocation location) : base(location)
        {
            this.Value = value;
        }
    }

    public class BoolLiteral : Literal
    {
        public override LiteralType Type => LiteralType.FloatLiteral;

        public bool Value { get; private init; }

        public BoolLiteral(bool value, SourceLocation location) : base(location)
        {
            this.Value = value;
        }
    }

    public class Identifier : Token
    {
        public override TokenType TokenCatagory => TokenType.Identifier;

        public string Name { get; private init; }

        public override string ToString()
        {
            return $"<Id: {Name}>";
        }

        public Identifier(string name, SourceLocation location) : base(location)
        {
            this.Name = name;
        }
    }
}
