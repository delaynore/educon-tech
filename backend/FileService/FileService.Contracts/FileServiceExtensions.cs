using FileService.Contracts.HttpCommunication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FileService.Contracts;

public static class FileServiceExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddFileServiceHttpClient(IConfiguration configuration, string section = "FileService")
        {
            services.Configure<FileServiceOptions>(configuration.GetSection(section));

            services.AddHttpClient<IFileServiceHttpClient, FileServiceHttpClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<FileServiceOptions>>().Value;

                client.BaseAddress = new Uri(options.BaseAddress);
                client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            });

            services.AddScoped<IFileServiceHttpClient, FileServiceHttpClient>();

            return services;
        }
    }
}
