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
    /// <summary>
    /// Lightweight HTTP server used to receive callback notifications from the
    /// POS system.  The server listens for checkout events and raises
    /// <see cref="OnCheckoutReceived"/> when a list of items is posted to the
    /// <c>/checkout</c> endpoint.
    /// </summary>
    public class PosCallbackServer : IDisposable
    {
        /// <summary>Underlying <see cref="HttpListener"/> instance.</summary>
        private readonly HttpListener _listener = new HttpListener();

        private CancellationTokenSource _cts;

        /// <summary>
        /// Fired when the POS system posts a checkout payload.  The event provides
        /// the list of items that were purchased.
        /// </summary>
        public event Action<List<CheckoutItem>> OnCheckoutReceived;

        /// <summary>
        /// Constructs the server and configures the listening prefix.
        /// </summary>
        /// <param name="prefix">URL prefix to listen on.</param>
        public PosCallbackServer(string prefix = "http://127.0.0.1:8080/visagent/")
        {
            _listener.Prefixes.Add(prefix);
        }

        /// <summary>
        /// Starts the HTTP listener and begins accepting requests.
        /// </summary>
        public void Start()
        {
            if (_listener.IsListening)
                return;

            _cts = new CancellationTokenSource();
            _listener.Start();
            Task.Run(() => LoopAsync(_cts.Token));
        }

        /// <summary>
        /// Internal loop that processes incoming HTTP requests until cancellation
        /// is requested.
        /// </summary>
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

        /// <summary>
        /// Stops the listener and releases resources.
        /// </summary>
        public void Dispose()
        {
            try { _cts?.Cancel(); } catch { }
            try { _listener?.Stop(); } catch { }
            try { _listener?.Close(); } catch { }
        }
    }
}
