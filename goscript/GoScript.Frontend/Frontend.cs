namespace GoScript.Frontend
{
    public static class Frontend
    {
        public static IEnumerable<Token> Lex(SourceFile file)
        {
            return new Lexer(file).Lex();
        }
    }
}
