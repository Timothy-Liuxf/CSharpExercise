using GoScript.Utils;

namespace GoScript.Frontend.Lex
{
    public class SourceFile
    {
        private SourceLocation location = new() { Line = 0, Column = 0 };
        public SourceLocation Location => location;

        private TextReader reader;

        public string? CurrentLine { get; private set; }

        public bool NextLine()
        {
            CurrentLine = reader.ReadLine();
            ++location.Line;
            location.Column = 0;
            return CurrentLine is not null;
        }

        public char? TopCharacter
        {
            get
            {
                if (CurrentLine is not null && location.Column <= CurrentLine.Length && location.Column > 0)
                {
                    return CurrentLine[location.Column - 1];
                }
                return null;
            }
        }

        public char? NextCharacter()
        {
            ++location.Column;
            return TopCharacter;
        }

        public SourceFile(TextReader reader)
        {
            this.reader = reader;
        }
    }
}
