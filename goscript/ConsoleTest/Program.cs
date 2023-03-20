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
        Console.WriteLine(' ' + string.Join(' ', tokens));
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
        Console.WriteLine(' ' + string.Join(' ', tokens));
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
            x + y % 4 + -a * (b - w) / z;
            ;

            """;
        var tokens = Frontend.Lex(new SourceFile(new StringReader(prog)));
        Console.WriteLine(' ' + string.Join(' ', tokens));

        var asts = Frontend.Parse(Frontend.Lex(new SourceFile(new StringReader(prog))));
        foreach (var ast in asts)
        {
            Console.WriteLine($"<---\n{ast.ToString()}-->");
        }
    }
}

{
    Console.WriteLine("===== Test Intepreter 0 =====\n\n");
    {
        var prog = """
            var x int32
            var y int32 = -6
            var z int32 = 32767
            var w int32 = 6;
            var a = x + y
            var b = a + z + w;
            var c int16 = 3
            var d int16 = 2000
            a
            b
            x
            y
            a + b
            a + b;
            1 + b + -3 * y * (a + b) / 3
            a % 5
            x
            - -(- -y)
            var e int16 = c * d + 5
            e
            var f int64 = 10
            var g int = 11
            var h = 12
            f + g + h
            """;
        var tokens = Frontend.Lex(new SourceFile(new StringReader(prog)));
        Console.WriteLine(' ' + string.Join(' ', tokens));

        var asts = Frontend.Translate(Frontend.Parse(Frontend.Lex(new SourceFile(new StringReader(prog)))));
        foreach (var ast in asts)
        {
            Console.WriteLine(ast.Attributes.Value ?? "No echo.");
        }
    }
}

{
    Console.WriteLine("===== Test Intepreter 1 =====\n\n");
    {
        var prog = """
            var x, y uint32 = 10, 11
            x + y
            x - y
            var a, b, c uint32
            a + b + c
            var d, e = 9, 8
            d + e
            {
                var x uint16 = 5
                var y uint32 = 6
                var z uint32 = 7
                x
                {
                    var z uint32 = 10
                    y + z
                }
            }
            x + y
            """;
        var tokens = Frontend.Lex(new SourceFile(new StringReader(prog)));
        Console.WriteLine(' ' + string.Join(' ', tokens));

        var asts = Frontend.Translate(Frontend.Parse(Frontend.Lex(new SourceFile(new StringReader(prog)))));
        foreach (var ast in asts)
        {
            Console.WriteLine(ast.Attributes.Value ?? "No echo.");
        }
    }
}

{
    Console.WriteLine("===== Test Intepreter 2 =====\n\n");
    {
        var prog = """
            var a bool = true
            var b = false
            var c bool
            a
            b
            c
            !a
            !b
            !(!!(c))
            !a && b || !c
            !a && (b || !c)
            !!a && (b || !c) && !!a
            """;
        var tokens = Frontend.Lex(new SourceFile(new StringReader(prog)));
        Console.WriteLine(' ' + string.Join(' ', tokens));

        var asts = Frontend.Translate(Frontend.Parse(Frontend.Lex(new SourceFile(new StringReader(prog)))));
        foreach (var ast in asts)
        {
            Console.WriteLine(ast.Attributes.Value ?? "No echo.");
        }
    }
}

{
    Console.WriteLine("===== Test Intepreter 3 =====\n\n");
    {
        var prog = """
            var x = 999
            var y int = 888
            x, y, z := 5, 6, 10
            x = 9
            x
            y
            z
            {
                var y = 4
                y = 10
                {
                    var x int
                    x = 12
                    x
                    z = x + 4
                }
                x + y
            }
            x
            y
            z
            z = 10
            {
                var z int = z + z
                z = z + z
                z
            }
            z
            """;
        var tokens = Frontend.Lex(new SourceFile(new StringReader(prog)));
        Console.WriteLine(' ' + string.Join(' ', tokens));

        var asts = Frontend.Translate(Frontend.Parse(Frontend.Lex(new SourceFile(new StringReader(prog)))));
        foreach (var ast in asts)
        {
            Console.WriteLine(ast.Attributes.Value ?? "No echo.");
        }
    }
}
