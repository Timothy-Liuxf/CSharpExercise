using GoScript.Utils;

namespace GoScript.Frontend
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
            ++this.location.Line;
            this.location.Column = 0;
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

        public bool NextCharacter()
        {
            ++this.location.Column;
            return CurrentLine is not null && location.Column <= CurrentLine.Length;
        }

        public SourceFile(TextReader reader)
        {
            this.reader = reader;
        }
    }
}
