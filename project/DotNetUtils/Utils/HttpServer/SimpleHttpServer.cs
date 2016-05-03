using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Web;

namespace DotNetUtils
{
    public class SimpleHttpServer
    {
        public const int DefaultPort = 55892;

        private readonly IList<Thread> _threadPool = new List<Thread>();
        private Thread _serverThread;
        private TcpListener _listener = null;
        private readonly IPAddress _ip;
        private readonly int _port;

        private bool _isActive = true;

        public Action<HttpRequestBase, HttpResponseBase> OnRequest;

        public SimpleHttpServer(IPAddress ip = null, int port = DefaultPort)
        {
            _ip = ip ?? new IPAddress(new byte[] { 127, 0, 0, 1 });
            this._port = port;
        }

        public void Stop()
        {
            _isActive = false;
            Thread.Sleep(1000);
            foreach (var thread in _threadPool)
            {
                thread.Abort();
            }
            _listener.Stop();
            Thread.Sleep(100);
            _serverThread.Abort();
        }

        public void Start()
        {
            _isActive = true;
            _serverThread = new Thread(ServerThread);
            _serverThread.Start();
        }

        private void ServerThread()
        {
            _listener = new TcpListener(_ip, _port);
            _listener.Start();
            while (_isActive)
            {
                var socket = _listener.AcceptTcpClient();
                var thread = _threadPool.FirstOrDefault(t => !t.IsAlive);
                if (thread == null)
                {
                    thread = new Thread(ProcessRequestTreadMain);
                    _threadPool.Add(thread);
                }
                thread.Start(new SimpleHttpRequest(socket, OnRequest));
                Thread.Sleep(1);
            }
        }

        private static void ProcessRequestTreadMain(object parameter)
        {
            ((SimpleHttpRequest)parameter).ProcessRequest();
        }
    }
}
