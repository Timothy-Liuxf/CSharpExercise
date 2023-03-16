using GoScript.Frontend;
using GoScript.Frontend.AST;
using GoScript.Frontend.Lexer;
using GoScript.Frontend.Parser;

namespace GoScript.Frontend
{
    public static class Frontend
    {
        public static IEnumerable<Token> Lex(SourceFile file)
        {
            return new Lexer.Lexer(file).Lex();
        }

        public static IEnumerable<ASTNode> Parse(IEnumerable<Token> tokens)
        {
            return new Parser.Parser(new TokenReader(tokens)).Parse();
        }
    }
}
