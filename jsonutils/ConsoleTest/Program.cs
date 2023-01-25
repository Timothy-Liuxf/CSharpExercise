using JsonUtils.Frontend;
using JsonUtils.Formatter;
using JsonUtils.Serializer;

string json =
@"{
    ""key1"": true,
    ""key2"": false,
    ""key3"": -1.6e+103,
    ""key4"": ""strVal"",
    ""key5"": null,
    ""arr"": [
        {
            ""key11""   :59,""key12"" :  666 ,""key13"":""""
            ,""key14"":0    ,    ""key15"":[],""key16"" : {}  ,
            ""key17"":{""key21"":{} ,},""key18"":[11,0x99ff,22,0X000,]
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

Console.WriteLine("===== Deserializer =====");
{
    var arr = Serializer.Deserialize<int?[]>(new StringReader(@"[1, null, 3]"));
    Console.Write("[ ");
    foreach (var elem in arr)
    {
        Console.Write($"{elem?.ToString() ?? "null"}, ");
    }
    Console.WriteLine("]");
}
Console.WriteLine();
