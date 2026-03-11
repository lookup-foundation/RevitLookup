using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;

namespace RevitLookup.Configuration;

public static class HttpClientConfiguration
{
    public static TBuilder ConfigureHttpClients<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddHttpClient("GitHubSource", client => client.BaseAddress = new Uri("https://api.github.com/repos/jeremytammik/RevitLookup/"));
        
        builder.Services.ConfigureHttpClientDefaults(clientBuilder => clientBuilder.RemoveAllLoggers());
        builder.Services.RemoveAll<IHttpMessageHandlerBuilderFilter>();
        
        return builder;
    }
}