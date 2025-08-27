using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using ViscoveryDemoPOS.Domain;

namespace ViscoveryDemoPOS.Services
{
    public class PosCallbackServer : IDisposable
    {
        private readonly HttpListener _listener = new HttpListener();
        private CancellationTokenSource _cts;
        public event Action<List<CheckoutItem>> OnCheckoutReceived;

        public PosCallbackServer(string prefix = "http://127.0.0.1:8080/visagent/")
        {
            _listener.Prefixes.Add(prefix);
        }

        public void Start()
        {
            if (_listener.IsListening) return;
            _cts = new CancellationTokenSource();
            _listener.Start();
            Task.Run(() => LoopAsync(_cts.Token));
        }

        private async Task LoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var ctx = await _listener.GetContextAsync().ConfigureAwait(false);
                if (ctx.Request.HttpMethod == "POST" && ctx.Request.Url.AbsolutePath.EndsWith("/checkout", StringComparison.OrdinalIgnoreCase))
                {
                    string body;
                    using (var sr = new System.IO.StreamReader(ctx.Request.InputStream, ctx.Request.ContentEncoding))
                        body = sr.ReadToEnd();

                    var items = JsonConvert.DeserializeObject<List<CheckoutItem>>(body);
                    OnCheckoutReceived?.Invoke(items);

                    var buffer = Encoding.UTF8.GetBytes("{\"status\":\"ok\"}");
                    ctx.Response.StatusCode = 200;
                    ctx.Response.ContentType = "application/json";
                    ctx.Response.OutputStream.Write(buffer, 0, buffer.Length);
                    ctx.Response.Close();
                }
                else
                {
                    ctx.Response.StatusCode = 404;
                    ctx.Response.Close();
                }
            }
        }

        public void Dispose()
        {
            try { _cts?.Cancel(); } catch { }
            try { _listener?.Stop(); } catch { }
            try { _listener?.Close(); } catch { }
        }
    }
}
