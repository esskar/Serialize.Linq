using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Serialize.Linq.Examples.RestClient
{
    public class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        public bool IsLoggingEnabled { get; set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (IsLoggingEnabled)
            {
                Console.WriteLine("Request:");
                Console.WriteLine(request.ToString());
                if (request.Content != null)
                {
                    Console.WriteLine(await request.Content.ReadAsStringAsync());
                }

                Console.WriteLine();
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (IsLoggingEnabled)
            {
                Console.WriteLine("Response:");
                Console.WriteLine(response.ToString());
                if (response.Content != null)
                {
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }

                Console.WriteLine();
            }

            return response;
        }
    }
}
