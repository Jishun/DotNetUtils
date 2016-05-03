using System;
using System.Collections.Specialized;
using System.IO;
using System.Net.Sockets;
using System.Web;

namespace DotNetUtils
{
    public class SimpleHttpRequest : HttpRequestBase
    {
        private readonly TcpClient _socket;
        private readonly Action<HttpRequestBase, HttpResponseBase> _onRequest;

        private string _method;
        private string _url;
        private string _protocolVersion;
        private NameValueCollection _headers;

        public SimpleHttpRequest(TcpClient socket, Action<HttpRequestBase, HttpResponseBase> onRequest)
        {
            _socket = socket;
            _onRequest = onRequest;
            InputStream = new BufferedStream(_socket.GetStream());
        }

        public override Stream InputStream { get; }

        public override NameValueCollection Headers => _headers;

        public override string ContentType => _headers["CONTENT-TYPE"];

        public override string UserAgent => _headers["USER-AGENT"];

        public override string[] AcceptTypes => _headers["ACCEPT"].Split(';');

        public override string HttpMethod => _method;

        public override Uri Url
        {
            get
            {
                var prefix = _protocolVersion.StartsWith("https", StringComparison.OrdinalIgnoreCase) ? "https://" : "http://";
                return new Uri(new Uri(prefix + _headers["HOST"]), _url);
            }
        }

        public void ProcessRequest()
        {
            try
            {
                var request = InputStream.ReadLine();
                var tokens = request.Split(' ');
                if (tokens.Length != 3)
                {
                    throw new HttpException(405, "Invalid http request line");
                }
                _method = tokens[0].ToUpper();
                _url = tokens[1];
                _protocolVersion = tokens[2];
                var headers = StreamExtensions.ReadHttpHeaders(InputStream);
                _headers = new NameValueCollection(headers.Count);
                foreach (var header in headers)
                {
                    _headers.Add(header.Key, header.Value);
                }
                var response = new SimpleHttpResponse(_socket);
                if (_onRequest != null)
                {
                    _onRequest(this, response);
                }
                else
                {
                    response.NoHandler();
                }
            }
            finally
            {
                _socket.Close();
            }
        }
    }

}
