using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using ThrottleHttpCalls.Middleware;

namespace ThrottleHttpCalls
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {


                    services.AddHttpClient("google", c=> { 
                        c.BaseAddress = new Uri("https://www.google.com/search");
                    })
                    .AddHttpMessageHandler(c=> new ThrottleRequestHandler(maxRequests: 2, period: TimeSpan.FromSeconds(10)));

                    services.AddHostedService<GoogleSearcher>();
                });
    }
}
