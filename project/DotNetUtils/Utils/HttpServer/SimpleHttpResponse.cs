using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;

namespace DotNetUtils
{

    public class SimpleHttpResponse : HttpResponseBase
    {
        private static readonly Encoding HttpHeaderEncoding = new UTF8Encoding(false);
        private readonly TcpClient _socket;
        private readonly Stream _outputStream;
        private readonly NameValueCollection _headers = new NameValueCollection();

        private MemoryStream _buffStream;

        public int BuffSize = 10240;

        public SimpleHttpResponse(TcpClient socket)
        {
            _socket = socket;
            _outputStream = _socket.GetStream();
        }

        public override Stream OutputStream
        {
            get
            {
                if (!BufferOutput)
                {
                    WriteHeaders(_outputStream);
                    return _outputStream;
                }
                return _buffStream ?? (_buffStream = new MemoryStream());
            }
        }

        public override bool BufferOutput { get; set; }
        public override int StatusCode { get; set; }

        public override NameValueCollection Headers => _headers;

        public override string ContentType
        {
            get { return _headers["CONTENT-TYPE"]; }
            set { _headers["CONTENT-TYPE"] = value; }
        }

        public override void Clear()
        {
            if (_buffStream != null)
            {
                _buffStream.Position = 0;
                _buffStream.SetLength(0);
            }
        }

        public override void End()
        {
            if (BufferOutput)
            {
#if (DEBUG)
                const string traceFileName = "header.txt";
                if (File.Exists(traceFileName))
                {
                    File.Delete(traceFileName);
                }
                WriteHeaders(new FileStream(traceFileName, FileMode.OpenOrCreate));
#endif
                WriteHeaders(_outputStream);
                _buffStream.Position = 0;
                _buffStream.CopyTo(_outputStream);
            }
            _socket.Close();
        }

        public void NoHandler()
        {
            using (var sw = new StreamWriter(_outputStream))
            {
                sw.WriteLine("HTTP/1.0 404 File not found");
                sw.WriteLine("Connection: close");
                sw.WriteLine("");
            }
        }
        private void WriteHeaders(Stream outStream)
        {
            using (var sw = new StreamWriter(outStream, HttpHeaderEncoding, BuffSize, true))
            {
                sw.WriteLine("HTTP/1.1 {0} {1}", StatusCode, ((HttpStatusCode)StatusCode));
                sw.WriteLine("Server: Test");
                sw.WriteLine("Date: {0}", DateTime.Now.ToString("r"));
                sw.WriteLine("Cache-Control: no-cache, no-store");
                sw.WriteLine("Content-Length: {0}", _buffStream.Length);
                sw.WriteLine("Pragma: no-cache");
                foreach (var key in Headers.AllKeys)
                {
                    sw.Write(key);
                    sw.Write(": ");
                    sw.WriteLine(Headers[key]);
                }
                sw.WriteLine("Connection: close");
                sw.WriteLine("");
                sw.Flush();
            }
        }
    }
}
