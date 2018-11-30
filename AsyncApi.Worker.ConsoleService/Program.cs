using System;
using System.Collections.Generic;
using System.Linq;
using AsyncApi.Worker.MessageBroker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AsyncApi.Worker.ConsoleService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            var startup = new Startup();

            startup.ConfigureServices(serviceCollection);
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            var app = serviceProvider.GetService<ConsoleApp>();

            AppDomain.CurrentDomain.ProcessExit += delegate (object sender, EventArgs eventArgs) { app.Closed(); };
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs eventArgs) { app.Closed(); };

            app.Run(args.FirstOrDefault());
        }
    }

    public class ConsoleApp
    {
        private readonly ILogger<ConsoleApp> _logger;
        private readonly IEnumerable<IListener> _listeners;
        private static string _name;

        public ConsoleApp(ILogger<ConsoleApp> logger, IEnumerable<IListener> listeners)
        {
            _logger = logger;
            _listeners = listeners;
        }

        public void Run(string name)
        {
            _name = name;

            Console.WriteLine($"Worker '{_name}' start...");
            _logger.LogInformation($"Worker '{_name}' start...");

            _logger.LogInformation($"Listeners '{_name}' started...");

            RunListeners(_listeners);

            _logger.LogInformation($"Worker '{_name}' started.");
            Console.WriteLine($"Worker '{_name}' started.");
        }

        public void Closed()
        {
            _logger.LogInformation($"Worker '{_name}' closed.");
            Console.WriteLine($"Worker '{_name}' closed.");
        }

        private void RunListeners(IEnumerable<IListener> listeners)
        {
            foreach (var listener in listeners)
            {
                listener.Run();
                _logger.LogInformation($"Listener for  worker '{_name}' with type '{listener.Type.ToString()}' started.");
            }
        }
    }
}