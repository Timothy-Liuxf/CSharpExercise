namespace GoScript.Utils
{
    public record struct SourceLocation
    {
        public /* required */ int Line { get; set; }
        public /* required */ int Column { get; set; }

        public override string ToString()
        {
            return $"({this.Line}, {this.Column})";
        }
    }
}
