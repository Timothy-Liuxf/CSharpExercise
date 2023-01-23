using JsonUtils.Frontend;
using JsonUtils.Formatter;

string json =
@"{
    ""key1"": true,
    ""key2"": false,
    ""key3"": 512,
    ""key4"": ""strVal"",
    ""arr"": [
        {
            ""key11""   :59,""key12"" :  666 ,""key13"":""""
            ,""key14"":0    ,    ""key15"":[],""key16"" : {}  ,
            ""key17"":{""key21"":{} ,},""key18"":[11,22,]
        },
        {
        }
    ]
}";

Console.WriteLine("===== Lexer =====");
{

    Console.WriteLine(json);
    var tokens = new FrontEnd(new StringReader(json)).Lex();
    foreach (var token in tokens)
    {
        Console.Write(token.ToString() + ' ');
    }
    Console.WriteLine();

    try
    {
        string missingQuote =
@"{
    ""key1: true
}";
        new FrontEnd(new StringReader(missingQuote)).Lex();
    }
    catch (SyntaxErrorException e)
    {
        Console.WriteLine(e.Message);
    }

    try
    {
        string integerOverflow =
@"{
    ""key"": 88888888888888888888
}";
        new FrontEnd(new StringReader(integerOverflow)).Lex();
    }
    catch (IntegerOverflowException e)
    {
        Console.WriteLine(e.Message);
    }

    try
    {
        string badIdentifier =
@"{
    ""key"": tr
}";
        new FrontEnd(new StringReader(badIdentifier)).Lex();
    }
    catch (SyntaxErrorException e)
    {
        Console.WriteLine(e.Message);
    }
}
Console.WriteLine();

Console.WriteLine("===== Parser =====");
{

    var frontend = new FrontEnd(new StringReader(json));
    var tokens = frontend.Lex();
    var ast = frontend.Parse();
    Console.WriteLine(ast.ToString());
}
Console.WriteLine();

Console.WriteLine("===== Formatter =====");
{

    var frontend = new FrontEnd(new StringReader(json));
    var tokens = frontend.Lex();
    var ast = frontend.Parse();
    var formatter = new Formatter(ast);
    Console.WriteLine(formatter.Format());
}
Console.WriteLine();
