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

Console.WriteLine("===== Serializer =====");
{
    string jsons;

    jsons = Serializer.Serialize("string");
    Console.WriteLine(jsons);

    jsons = Serializer.Serialize(null);
    Console.WriteLine(jsons);

    int? nullInt = null;
    jsons = Serializer.Serialize(nullInt);
    Console.WriteLine(jsons);

    jsons = Serializer.Serialize(4.3m);
    Console.WriteLine(jsons);

    jsons = Serializer.Serialize(true);
    Console.WriteLine(jsons);

    var obj = new TestSerializeObjectType()
    {
        Name = "Tom",
        Age = 18,
        Married = true,
        LuckyNumber = null,
        Job = TestSerializeObjectType.JobType.Programmer,
        HairCount = -1,
        Salary = 3555.98m,
        Assets = 1e6m,
        Expenditures = new decimal?[] { 1.00m, null, 23.9m },
        BMI = 21.65,
        Children = new TestSerializeObjectType.ChildType[]
        {
            new TestSerializeObjectType.ChildType()
            {
                Name = "Mary",
                Age = 5,
            },
            new TestSerializeObjectType.ChildType()
            {
                Name = "Jack",
                Age = 3,
            },
        },
    };
    jsons = Serializer.Serialize(obj);
    Console.WriteLine(jsons);
    Console.WriteLine(new Formatter(new FrontEnd(new StringReader(jsons)).Parse()).Format());
}
Console.WriteLine();

Console.WriteLine("===== Deserializer with Escaping =====");
{
    var str = Serializer.Deserialize<string>(new StringReader(@"""01234\b\b5\n\t0123\u007B"""));
    Console.WriteLine(str);

    try
    {
        Serializer.Deserialize<string>(new StringReader(@"""01234\d"""));
    }
    catch (SyntaxErrorException ex)
    {
        Console.WriteLine($"{ex.ToString()}");
    }
    Console.WriteLine();

    try
    {
        Serializer.Deserialize<string>(new StringReader(@"""01234\dsss"""));
    }
    catch (SyntaxErrorException ex)
    {
        Console.WriteLine($"{ex.ToString()}");
    }
    Console.WriteLine();

    try
    {
        Serializer.Deserialize<string>(new StringReader(@"""01234\u889"""));
    }
    catch (SyntaxErrorException ex)
    {
        Console.WriteLine($"{ex.ToString()}");
    }
    Console.WriteLine();

    try
    {
        Serializer.Deserialize<string>(new StringReader(@"""01234\u88RB"""));
    }
    catch (SyntaxErrorException ex)
    {
        Console.WriteLine($"{ex.ToString()}");
    }
    Console.WriteLine();
}
Console.WriteLine();

Console.WriteLine("===== Serializer with Escaping =====");
{
    string jsons;

    var str = "string\b\f\\\"\t哈哈嘻嘻" + (char)30;
    jsons = Serializer.Serialize(str, true);
    Console.WriteLine(jsons);
    jsons = Serializer.Serialize(str, false);
    Console.WriteLine(jsons);
}
Console.WriteLine();

class TestSerializeObjectType
{
    public enum JobType
    {
        None = 0,
        Programmer = 1,
    }

    [JsonSerializeOption(key: "name", required: true)]
    public string Name { get; set; } = "";

    [JsonSerializeOption(key: "age", required: true)]
    public int Age { get; set; } = 0;

    [JsonSerializeOption(key: "married", required: true)]
    public bool Married { get; set; } = false;

    [JsonSerializeOption(key: "lucky-number", required: true)]
    public int? LuckyNumber { get; set; } = 0;

    [JsonSerializeOption(key: "job", required: true)]
    public JobType Job { get; set; } = JobType.None;

    [JsonSerializeOption(key: "hair-count", required: false, defaultValue: -1)]
    public long HairCount { get; set; } = 0;

    [JsonSerializeOption(key: "salary", required: true)]
    public decimal Salary { get; set; } = 0.0m;

    [JsonSerializeOption(key: "assets", required: true)]
    public decimal Assets { get; set; } = 0.0m;

    [JsonSerializeOption(key: "expenditures", required: true)]
    public decimal?[] Expenditures { get; set; } = { };

    [JsonSerializeOption(key: "bmi", required: true)]
    public double BMI { get; set; } = 0.0d;

    public class ChildType
    {
        [JsonSerializeOption(key: "name", required: true)]
        public string Name { get; set; } = "";

        [JsonSerializeOption(key: "age", required: true)]
        public int Age { get; set; } = 0;
    }

    [JsonSerializeOption(key: "children", required: true)]
    public ChildType?[] Children { get; set; } = { };
}
