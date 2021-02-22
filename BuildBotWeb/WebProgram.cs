namespace BuildBotWeb
{
    using BuildBotWeb.GrpcHistory.v1;
    using Grpc.Net.Client;
    using Grpc.Net.Client.Web;
    using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class WebProgram
    {
        private const string historyUrl = "https://localhost:5001";

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddSingleton(services =>
            {
                // Create a gRPC-Web channel pointing to the backend server
                var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()));
                var channel = GrpcChannel.ForAddress(historyUrl, new GrpcChannelOptions { HttpClient = httpClient });

                // Now we can instantiate gRPC clients for this channel
                return new History.HistoryClient(channel);
            });

            await builder.Build().RunAsync();
        }
    }
}
