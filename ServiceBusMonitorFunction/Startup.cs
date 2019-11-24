using System;
using ServiceBusMonitorFunction.Services;
using ServiceBusMonitorFunction.Services.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace ServiceBusMonitorFunction
{
    internal class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder) => builder.AddDependencyInjection(ConfigureServices);

        private void ConfigureServices(IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            var elasticConnectionString =
                config.GetConnectionString("ElasticConnection") ??
                config.GetValue<string>("Values:ElasticConnection") ??
                config.GetValue<string>("ElasticConnection") ??
                Environment.GetEnvironmentVariable("ElasticConnection");

            Console.WriteLine($"Elastic connection: {elasticConnectionString}");
            services.AddScoped<IElasticService, ElasticService>(sp => new ElasticService(elasticConnectionString));
            services.AddScoped<IAlertsService, AlertsService>();
            services.AddScoped<IMailQueueClient, MailQueueClient>();
            services.AddSingleton<IConfiguration>(config);
        }
    }
}