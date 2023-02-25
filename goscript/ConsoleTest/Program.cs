using GoScript.Frontend;

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
