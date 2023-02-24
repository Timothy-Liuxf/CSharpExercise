namespace GoScript.Utils
{
    public record struct SourceLocation
    {
        public /* required */ int Line { get; init; }
        public /* required */ int Column { get; init; }

        public override string ToString()
        {
            return $"({this.Line}, {this.Column})";
        }
    }
}
