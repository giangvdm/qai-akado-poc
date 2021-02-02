using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace akaCommon
{
    public delegate Task ProcessPostDataCallback(string data);
    public class AkaFocusHttpServer : IDisposable
    {
        private HttpListener _listener;
        private const string _pageData = "OK";
        private readonly ProcessPostDataCallback _processPostDataCallback;

        public AkaFocusHttpServer(string url, ProcessPostDataCallback processPostDataCallback)
        {
            this.Url = !string.IsNullOrEmpty(url) ? url : "http://localhost:33450/";
            _processPostDataCallback = processPostDataCallback;
        }

        public string Url { get; set; }
        public async Task HandleIncomingConnections()
        {
            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (true)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext context = await _listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = context.Request;
                HttpListenerResponse resp = context.Response;

#if DEBUG
                // Print out some info about the request
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();
#endif
                if (req.HttpMethod == "POST" && _processPostDataCallback != null)
                {
                    // Get the data from the HTTP stream
                    var body = new StreamReader(context.Request.InputStream).ReadToEnd();
                    await _processPostDataCallback(body);
                }

                // Write the response info
                byte[] data = Encoding.UTF8.GetBytes(_pageData);
                resp.StatusCode = 200;
                resp.ContentType = "text/html";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                // Write out to the response stream (asynchronously), then close it
                await resp.OutputStream.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
                resp.Close();
            }
        }

        public async Task Start()
        {
            // Create a Http server and start listening for incoming connections
            _listener = new HttpListener();
            _listener.Prefixes.Add(this.Url);
            _listener.Start();
#if DEBUG
            Console.WriteLine("Listening for connections on {0}", this.Url);
#endif
            // Handle requests
            await HandleIncomingConnections();

            // Close the listener
            _listener.Close();
        }

        #region IDisposable Support
        private bool disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing && _listener != null && _listener is IDisposable)
                {
                    // Dispose managed state (managed objects).
                    (_listener as IDisposable).Dispose();
                    _listener = null;
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
