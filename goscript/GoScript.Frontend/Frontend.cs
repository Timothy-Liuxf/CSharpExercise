using GoScript.Frontend.AST;

namespace GoScript.Frontend
{
    public static class Frontend
    {
        public static IEnumerable<Token> Lex(SourceFile file)
        {
            return new Lexer(file).Lex();
        }

        public static IEnumerable<ASTNode> Parse(IEnumerable<Token> tokens)
        {
            return new Parser(new TokenReader(tokens)).Parse();
        }
    }
}
