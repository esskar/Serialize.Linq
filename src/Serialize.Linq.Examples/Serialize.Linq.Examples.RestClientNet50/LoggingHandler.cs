using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Serialize.Linq.Examples.RestClientNet50
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
                    Console.WriteLine(await request.Content.ReadAsStringAsync(cancellationToken));
                }

                Console.WriteLine();
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (IsLoggingEnabled)
            {
                Console.WriteLine("Response:");
                Console.WriteLine(response.ToString());
                Console.WriteLine(await response.Content.ReadAsStringAsync(cancellationToken));

                Console.WriteLine();
            }

            return response;
        }
    }
}
