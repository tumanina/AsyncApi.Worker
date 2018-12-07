using System.IO;
using AsyncApi.Worker.MessageBroker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace AsyncApi.Worker.ConsoleService
{
    public class Startup
    {
        IConfigurationRoot Configuration { get; }

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
        }
        
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddServices(Configuration);

            serviceCollection.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageTemplates = true,
                    CaptureMessageProperties = true
                });
            });
            NLog.LogManager.LoadConfiguration("nlog.config");

            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceCollection.AddTransient<ConsoleApp>(t => new ConsoleApp(serviceProvider.GetService<ILogger<ConsoleApp>>(), 
                serviceCollection.BuildServiceProvider().GetServices<IListener>()));

            serviceCollection.AddListeners(serviceProvider, Configuration);
        }
    }
}
