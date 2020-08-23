using Microsoft.AspNetCore.Builder;

public static class ClientConfigurationExtension
    {
        public static IApplicationBuilder UseClientConfiguration(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseMiddleware<ClientConfigurationMiddleware>();
        }
    }