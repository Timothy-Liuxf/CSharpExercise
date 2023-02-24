using GoScript.Frontend;

var prog = """
    {
        >= <= >> << == != && || & =
        [+-*/()%~] =
    }
    """;

var tokens = new Lexer(new SourceFile(new StringReader(prog))).Lex();
Console.WriteLine(string.Join(' ', tokens));
