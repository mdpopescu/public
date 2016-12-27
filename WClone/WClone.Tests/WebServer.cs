using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WClone.Tests
{
    public class WebServer
    {
        public WebServer(int port)
        {
            this.port = port;
        }

        public void Add(string path, string contents)
        {
            dict[path] = contents;
        }

        public IDisposable Start()
        {
            var server = new HttpListener();
            server.Prefixes.Add($"http://localhost:{port}/");
            server.Start();

            var cts = new CancellationTokenSource();
            var task = Task.Run(() => ProcessRequests(server, cts.Token), cts.Token);

            return Disposable.Create(() =>
            {
                cts.Cancel();
                task.Wait(TimeSpan.FromMilliseconds(10));
                cts.Dispose();

                server.Stop();
                server.Close();
            });
        }

        //

        private readonly Dictionary<string, string> dict = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);

        private readonly int port;

        private void ProcessRequests(HttpListener server, CancellationToken token)
        {
            while (server.IsListening && !token.IsCancellationRequested)
            {
                var requestTask = Task.Run(() => server.GetContext(), token);
                try
                {
                    ProcessRequest(requestTask.Result, token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private void ProcessRequest(HttpListenerContext context, CancellationToken token)
        {
            var response = context.Response;

            var key = context.Request.Url.AbsolutePath;
            if (!dict.ContainsKey(key))
            {
                response.StatusCode = 404;
                return;
            }

            Task
                .Run(() => SendResponse(key, response), token)
                .Wait(token);
        }

        private void SendResponse(string key, HttpListenerResponse response)
        {
            var contents = dict[key];
            var buffer = Encoding.UTF8.GetBytes(contents);

            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
        }
    }
}