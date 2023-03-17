using GoScript.Frontend;
using GoScript.Frontend.Lex;

{
    Console.WriteLine("===== Test Lexer =====\n\n");
    {
        var prog = """
        {
            >= <= >> << == != && || & =
            [+-*/()%~] =
        }
        """;

        var tokens = Frontend.Lex(new SourceFile(new StringReader(prog)));
        Console.WriteLine(string.Join(' ', tokens));
    }

    {
        var prog = """
        package main

        import "fmt"

        func test1(x float64) float64 {
            return x * 3.65 - 9.8e3
        }

        func test2() {
            fmt.Println("hello, \n world!\n")
        }

        func _s_1() {}

        func main() {
            var x int32 = 10
            for (var i int32 = 8; i < x; i++) {
                x--
            }
            test1(float64(0x888a))
            test2()
            return
        }
        """;

        var tokens = Frontend.Lex(new SourceFile(new StringReader(prog)));
        Console.WriteLine(string.Join(' ', tokens));
    }
}

{
    Console.WriteLine("===== Test Parser =====\n\n");
    {
        var prog = """
            var x int32
            var y int32 = 6
            var z int32;
            var w int32 = 6;
            var a = x + y
            var b = a + z + w;
            a + b
            x + y + a + b;
            ;

            """;
        var tokens = Frontend.Lex(new SourceFile(new StringReader(prog)));
        Console.WriteLine(string.Join(' ', tokens));

        var asts = Frontend.Parse(Frontend.Lex(new SourceFile(new StringReader(prog))));
        foreach (var ast in asts)
        {
            Console.WriteLine($"<---\n{ast.ToString()}-->");
        }
    }
}

{
    Console.WriteLine("===== Test Intepreter =====\n\n");
    {
        var prog = """
            var x int32
            var y int32 = 6
            var z int32;
            var w int32 = 6;
            var a = x + y
            var b = a + z + w;
            a
            b
            x
            y
            a + b
            a + b;
            1 + b + 3 * y * (a + b) / 3
            a % 5
            x
            (y)
            """;
        var tokens = Frontend.Lex(new SourceFile(new StringReader(prog)));
        Console.WriteLine(string.Join(' ', tokens));

        var asts = Frontend.Translate(Frontend.Parse(Frontend.Lex(new SourceFile(new StringReader(prog)))));
        foreach (var ast in asts)
        {
            Console.WriteLine(ast.Attributes.Value ?? "No echo.");
        }
    }
}
