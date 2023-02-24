using GoScript.Frontend;

var prog = """
    {
        >= <= >> << == != && || & =
        [+-*/()%~] =
    }
    """;

var tokens = Frontend.Lex(new SourceFile(new StringReader(prog)));
Console.WriteLine(string.Join(' ', tokens));
