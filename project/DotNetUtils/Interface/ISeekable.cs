namespace DotNetUtils
{
    public interface ISeekable
    {
        string Source { get; }
        int Position { get; set; }
        int Line { get; }
        int Column { get; }
        int PreviousPosition { get; }
        int PreviousLine { get; }
        int PreviousColumn { get; }
        bool Eof { get; }
        int Length { get; }
        string Left { get; }

        ISeekable Substring(int startIndex, int length);
        ISeekable Substring(int startIndex);
        bool StartsWith(string match);
        void Back(int count = 1);
        int PeekChar();
        int ReadChar();
        string ReadTo(string match, bool trimPattern = true);
        string ReadTo(bool trimPattern, string escape, params string[] terminitors);
        string ReadTo(bool trimPattern, out string matched, string escape, params string[] terminitors);
    }
}
