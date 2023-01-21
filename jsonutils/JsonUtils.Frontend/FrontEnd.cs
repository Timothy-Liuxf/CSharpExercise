using JsonUtils.Frontend.AST;

namespace JsonUtils.Frontend
{
    public class FrontEnd
    {
        private SourceManager sourceManager;
        private Lexer lexer;

        public IList<Token> Lex()
        {
            return lexer.Lex(sourceManager);
        }

        public ASTNode Parse(IList<Token> tokens)
        {
            return new Parser(new TokenReader(tokens), lexer.EndLocation).Parse();
        }

        public FrontEnd(TextReader reader)
        {
            sourceManager = new SourceManager(reader);
            lexer = new Lexer();
        }
    }
}
