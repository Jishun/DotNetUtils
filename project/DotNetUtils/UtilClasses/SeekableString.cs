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
        private int _position;
        private int _line;
        private int _column;
        private bool _lineColumnNumberAccurate;

        [DebuggerStepThrough]
        public SeekableString(string src, LineBreakOption lineBreakOption = LineBreakOption.Nonspecified)
        {
            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
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
                return Eof? String.Empty : _src.Substring(Position, Length - Position);
            }
        }

        public int Position
        {
            get { return _position; }
            set
            {
                _position = value;
                _lineColumnNumberAccurate = false;
            }
        }

        public int Line
        {
            get
            {
                if (!_lineColumnNumberAccurate)
                {
                    CalculateLineColumnNumbers();
                } 
                return _line;
            }
            private set { _line = value; }
        }

        public int Column
        {
            get
            {
                if (!_lineColumnNumberAccurate)
                {
                    CalculateLineColumnNumbers();
                }
                return _column;
            }
            private set { _column = value; }
        }

        public int PreviousPosition { get; private set; }
        public int PreviousLine { get; private set; }
        public int PreviousColumn { get; private set; }

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
                _position--;
                Column--;
                if (Column < 0)
                {
                    _lineColumnNumberAccurate = false;
                }
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
            PreviousPosition = Position;
            PreviousColumn = Column;
            PreviousLine = Line;
            if (Position < _src.Length)
            {
                var ret = _src[_position++];
                Column++;
                if (ret == '\n' || (ret == '\r' && PeekChar() == -1))
                {
                    Line++;
                    Column = 0;
                }
                return ret;
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

        public string ReadTo(bool trimPattern, out string matched, string escape, params string[] terminators)
        {
            if (terminators.Length == 0)
            {
                throw new ArgumentNullException(nameof(terminators));
            }
            if (terminators.Contains(escape))
            {
                throw new ArgumentException("Escape cannot be the same with terminator");
            }
            if (escape == String.Empty)
            {
                escape = null;
            }
            PreviousPosition = Position;
            PreviousColumn = Column;
            PreviousLine = Line;
            var sb = new StringBuilder();
            var length = terminators.Max(t => t.Length);
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
                    _position += escape.Length;
                    Column += escape.Length;
                    str = _src.Substring(Position, length);
                    escaping = true;
                }
                var escaped = false;
                foreach (var terminitor in escape == null ? terminators : terminators.Append(escape))
                {
                    if (str.StartsWith(terminitor))
                    {
                        _position += terminitor.Length;
                        Column += terminitor.Length;
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
                    Column += escape.Length;
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

        private void CalculateLineColumnNumbers()
        {
            for (var i = 0; i < _position; i++)
            {
                var c = _src[i];
                var nextC = i < _src.Length - 1 ? _src[i + 1] : -1;
                var str = nextC == -1 || (nextC != '\r' && nextC != '\n') ? c.ToString() : (c.ToString() + (char)nextC);
                switch (str)
                {
                    case "\r\n":
                    case "\n":
                    case "\r":
                        i++;
                        _line++;
                        _column = 0;
                        break;
                    default:
                        _column++;
                        break;
                }
            }
            _lineColumnNumberAccurate = true;
        }

        private void AppendCharWithTranslatingLineBreak(StringBuilder sb)
        {
            var c = _src[_position++];
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
                    _position++;
                    Line++;
                    Column=0;
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
                    Line++;
                    Column=0;
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
                    
                    Line++;
                    Column=0;
                    break;
                default:
                    Column++;
                    str = c.ToString();
                    break;
            }
            sb.Append(str);
        }

        public override string ToString()
        {
            return _src;
        }
    }
}
