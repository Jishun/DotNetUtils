using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DotNetUtils
{
    [Flags]
    public enum LineBreakOption
    {
        Nonspecified = 0,
        CrLfToLf = 1,
        CrLfToCr = 2,
        LfToCrLf = 4,
        LfToCr = 8,
        CrToCrLf = 16,
        CrToLf = 32,
    }
    public class SeekableString : ISeekable
    {
        private readonly string _src;
        private readonly LineBreakOption _lineBreakOption;

        [DebuggerStepThrough]
        public SeekableString(string src, LineBreakOption lineBreakOption = LineBreakOption.Nonspecified)
        {
            if (src == null)
            {
                throw new ArgumentNullException("src");
            }
            _src = src;
            _lineBreakOption = lineBreakOption;
        }
        public string Source
        {
            [DebuggerStepThrough]
            get { return _src; }
        }
        public string Left
        {
            [DebuggerStepThrough]
            get
            {
                return Eof? _src.Substring(Position, Length - Position) : String.Empty;
            }
        }

        public int Position { get; set; }

        public bool Eof
        {
            [DebuggerStepThrough]
            get { return Position >= _src.Length; }
        }

        public int Length
        {
            get { return _src.Length; }
        }

        public bool StartsWith(string match)
        {
            return _src.StartsWith(match);
        }

        public void Back(int count = 1)
        {
            if (Position > 0)
            {
                Position--;
            }
        }

        public int PeekChar()
        {
            if (Position < _src.Length)
            {
                return _src[Position];
            }
            return -1;
        }

        public int ReadChar()
        {
            if (Position < _src.Length)
            {
                return _src[Position++];
            }
            return -1;
        }
        public ISeekable Substring(int startIndex, int length)
        {
            return new SeekableString(_src.Substring(startIndex, length), _lineBreakOption);
        }

        public ISeekable Substring(int startIndex)
        {
            return new SeekableString(_src.Substring(startIndex, _src.Length - startIndex), _lineBreakOption);
        }

        public string ReadTo(string match, bool trimPattern = true)
        {
            return ReadTo(trimPattern, null, match);
        }

        public string ReadTo(bool trimPattern, string escape, params string[] terminitors)
        {
            string matched = null;
            return ReadTo(trimPattern, out matched, escape, terminitors);
        }

        public string ReadTo(bool trimPattern, out string matched, string escape, params string[] terminitors)
        {
            if (escape == String.Empty)
            {
                throw new ArgumentNullException("escape");
            }
            if (terminitors.Length == 0)
            {
                throw new ArgumentNullException("terminitors");
            }
            if (terminitors.Contains(escape))
            {
                throw new ArgumentException("Escape cannot be the same with terminitor");
            }
            var sb = new StringBuilder();
            var length = terminitors.Max(t => t.Length);
            if (escape != null)
            {
                length = Math.Max(length, escape.Length);
            }
            while (!Eof)
            {
                length = Math.Min(length, _src.Length - Position);
                var str = _src.Substring(Position, length);
                var escaping = false;
                if (escape != null && str.StartsWith(escape))
                {
                    Position += escape.Length;
                    str = _src.Substring(Position, length);
                    escaping = true;
                }
                var escaped = false;
                foreach (var terminitor in escape == null ? terminitors : terminitors.Append(escape))
                {
                    if (str.StartsWith(terminitor))
                    {
                        Position += terminitor.Length;
                        if (escaping || !trimPattern)
                        {
                            sb.Append(terminitor);
                            escaped = true;
                        }
                        if (!escaping)
                        {
                            matched = terminitor;
                            return sb.ToString();
                        }
                    }
                }
                if (escaping && !escaped)
                {
                    sb.Append(escape);
                }
                if (!escaping)
                {
                    AppendCharWithTranslatingLineBreak(sb);
                }
            }
            matched = null;
            return sb.ToString();
        }

        public string ReadTo(int index)
        {
            return _src.Substring(Position, index - Position);
        }

        public override string ToString()
        {
            return _src;
        }

        private void AppendCharWithTranslatingLineBreak(StringBuilder sb)
        {
            var c = _src[Position++];
            var nextC = PeekChar();
            var str = nextC == -1 || (nextC != '\r' && nextC != '\n') ? c.ToString() : (c.ToString() + (char) nextC);
            switch (str)
            {
                case "\r\n":
                    if (_lineBreakOption.HasFlag(LineBreakOption.CrLfToCr))
                    {
                        str = "\r";
                    }
                    else if (_lineBreakOption.HasFlag(LineBreakOption.CrLfToLf))
                    {
                        str = "\n";
                    }
                    Position++;
                    break;
                case "\n":
                    if (_lineBreakOption.HasFlag(LineBreakOption.LfToCr))
                    {
                        str = "\r";
                    }
                    else if (_lineBreakOption.HasFlag(LineBreakOption.LfToCrLf))
                    {
                        str = "\r\n";
                    }
                    break;
                case "\r":
                    if (_lineBreakOption.HasFlag(LineBreakOption.CrToCrLf))
                    {
                        str = "\r\n";

                    }
                    else if (_lineBreakOption.HasFlag(LineBreakOption.CrToLf))
                    {
                        str = "\n";
                    }
                    break;
                default:
                    str = c.ToString();
                    break;
            }
            sb.Append(str);
        }
    }
}
