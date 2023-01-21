namespace JsonUtils.Frontend
{
    public struct SourceLocation
    {
        public int Line { get; set; }
        public int Column { get; set; }

        public SourceLocation(int line, int column)
        {
            Line = line;
            Column = column;
        }
    }

    internal class SourceManager
    {
        private SourceLocation location = new(0, 0);
        public SourceLocation Location => location;
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

        private TextReader reader;

        public SourceManager(TextReader reader)
        {
            this.reader = reader;
        }
    }
}
