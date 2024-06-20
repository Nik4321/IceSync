using IceSync.Data;
using IceSync.Options;
using IceSync.Services;
using IceSync.Services.Clients;
using IceSync.Services.Sync;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IceSync.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IHttpClientBuilder AddUniversalLoaderHttpClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<UniversalLoaderOptions>(configuration.GetSection("UniversalLoader"));

            services.AddTransient<AuthorizationHeaderHandler>();

            services.AddScoped<IUniversalLoaderClient, UniversalLoaderClient>();

            return services.AddHttpClient<IUniversalLoaderClient, UniversalLoaderClient>("UniversalLoaderClient", (provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<UniversalLoaderOptions>>().Value;

                client.BaseAddress = new Uri(options.BaseUrl);
            }).AddHttpMessageHandler<AuthorizationHeaderHandler>();
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUniversalLoaderService, UniversalLoaderService>();

            services.AddHostedService<WorkflowSyncService>();

            return services;
        }

        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var dbConnectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<IceSyncDbContext>(options =>
                options.UseSqlServer(dbConnectionString, builder =>
                {
                    builder.EnableRetryOnFailure(3);
                }));
        }
    }
}
