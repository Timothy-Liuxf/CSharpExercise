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

        public FrontEnd(TextReader reader)
        {
            sourceManager = new SourceManager(reader);
            lexer = new Lexer();
        }
    }
}
