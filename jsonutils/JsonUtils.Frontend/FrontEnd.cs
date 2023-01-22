using JsonUtils.Frontend.AST;

namespace JsonUtils.Frontend
{
    public class FrontEnd
    {
        private Lexer lexer;

        public IEnumerable<Token> Lex()
        {
            return lexer.Lex();
        }

        public ASTNode Parse()
        {
            return new Parser(new TokenReader(lexer.Lex()), lexer.EndLocation).Parse();
        }

        public FrontEnd(TextReader reader)
        {
            lexer = new Lexer(new SourceManager(reader));
        }
    }
}
