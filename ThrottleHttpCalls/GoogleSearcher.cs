using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ThrottleHttpCalls
{
    public class GoogleSearcher : BackgroundService
    {
        private readonly ILogger<GoogleSearcher> _logger;
        private readonly HttpClient googleSearch;

        public GoogleSearcher(ILogger<GoogleSearcher> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            this.googleSearch = clientFactory.CreateClient("google");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                var result = await googleSearch.GetStringAsync("?q=Who%20else%20doesn%27t%20want%20the%20Springboks%20to%20change%20their%20style%3F");

                _logger.LogInformation("Request at: {time}", DateTimeOffset.Now.ToUniversalTime());
            }
        }
    }
}
