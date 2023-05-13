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
            1 + 2 + z + 4 + 5
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
            z * 9
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
    Console.WriteLine("===== Test Intepreter 4 =====\n\n");
    {
        var prog = """
            x, y, z := 10, -10, 20
            x > y
            x < y
            x == y
            x != y
            x >= x
            x <= x
            z == 20
            z != 20
            z > -6
            z <= -8
            10 > -96
            10 <= -50
            x + y > 23 + 89 + z + 56 + 89
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
    Console.WriteLine("===== Test Intepreter 5 =====\n\n");
    {
        var prog = """
            x, y := true, false
            var z bool = false
            var w = x != y
            w
            x == y
            z == w
            y == z == w
            x == true
            x == false
            true == false
            true == true
            true == x
            false == x
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
    Console.WriteLine("===== Test Intepreter 6 =====\n\n");
    {
        var prog = """
            var x int = 20
            var y int = 3
            var z = y + 17
            if x != z {
                y = 888
                true
            } else if x == y {
                x = 666
                false
            } else if false {
            } else {
                x = 777
                false
            }
            x
            y
            z
            if x == 777 {
                true
            }
            if x != 777 {
                false
            } else if x == 777 {
                true
            }
            x
            """;
        var tokens = Frontend.Lex(new SourceFile(new StringReader(prog)));
        Console.WriteLine(' ' + string.Join(' ', tokens));
        var astsParsed = Frontend.Parse(Frontend.Lex(new SourceFile(new StringReader(prog))));
        Console.WriteLine(string.Join("", astsParsed));

        var asts = Frontend.Translate(Frontend.Parse(Frontend.Lex(new SourceFile(new StringReader(prog)))));
        foreach (var ast in asts)
        {
            Console.WriteLine(ast.Attributes.Value ?? "No echo.");
        }
    }
}

{
    Console.WriteLine("===== Test Intepreter 7 =====\n\n");
    {
        var prog = """
            sum := 0
            for i := 0; i < 10; i = i + 1 {
                sum = sum + i
            }
            sum
            sum = 0
            i := 0
            for i < 10 {
                sum = sum + i
                i = i + 1
            }
            sum
            for {
                sum = 888
                break
                sum = 999
            }
            sum
            sum = 0
            i = 777
            for i := 0; i < 10; i = i + 1 {
                sum = sum + i
                if i == 5 {
                    break
                } else {
                    continue
                }
                sum = 888
            }
            sum
            i
            for i := 0; i < 10; i = i + 1 {
                i := 1
            }
            for i := 0; i < 10; i = i + 1 {
                var i = 1
            }
            """;
        var tokens = Frontend.Lex(new SourceFile(new StringReader(prog)));
        Console.WriteLine(' ' + string.Join(' ', tokens));
        var astsParsed = Frontend.Parse(Frontend.Lex(new SourceFile(new StringReader(prog))));
        Console.WriteLine(string.Join("", astsParsed));

        var asts = Frontend.Translate(Frontend.Parse(Frontend.Lex(new SourceFile(new StringReader(prog)))));
        foreach (var ast in asts)
        {
            Console.WriteLine(ast.Attributes.Value ?? "No echo.");
        }
    }
}

{
    Console.WriteLine("===== Test Intepreter 8 =====\n\n");
    {
        var prog = """
            n, res := 80, 0
            f0 := 0
            f1 := 1
            for i := 2; i <= n; i = i + 1 {
                res = f0 + f1;
                f0 = f1;
                f1 = res;
            }
            res
            """;
        var tokens = Frontend.Lex(new SourceFile(new StringReader(prog)));
        Console.WriteLine(' ' + string.Join(' ', tokens));
        var astsParsed = Frontend.Parse(Frontend.Lex(new SourceFile(new StringReader(prog))));
        Console.WriteLine(string.Join("", astsParsed));

        var asts = Frontend.Translate(Frontend.Parse(Frontend.Lex(new SourceFile(new StringReader(prog)))));
        foreach (var ast in asts)
        {
            Console.WriteLine(ast.Attributes.Value ?? "No echo.");
        }
    }
}

{
    Console.WriteLine("===== Test Intepreter 9 =====\n\n");
    {
        var prog = """
            var x func ()
            var y func (bool) int
            var z func (int16)
            var w func () (uint, bool, uint16) = func () (uint, bool, uint16) {
            }
            f1 := func (x int, y, z func () int) {
                x + x
                z
            }
            var f2 func (int, bool, bool) int32
            f2 = func (x int, y, z bool) int32 {
                y && z
                x + x
                f1 := 5
            }
            x
            y
            z
            w
            f1
            f2
            """;
        var tokens = Frontend.Lex(new SourceFile(new StringReader(prog)));
        Console.WriteLine(' ' + string.Join(' ', tokens));
        var astsParsed = Frontend.Parse(Frontend.Lex(new SourceFile(new StringReader(prog))));
        Console.WriteLine(string.Join("", astsParsed));

        var asts = Frontend.Translate(Frontend.Parse(Frontend.Lex(new SourceFile(new StringReader(prog)))));
        foreach (var ast in asts)
        {
            Console.WriteLine(ast.Attributes.Value ?? "No echo.");
        }
    }
}
