using System.Collections.Generic;
using System.Linq;
using AsyncApi.Worker.Configuration;
using AsyncApi.Worker.MessageBroker;
using AsyncApi.Worker.Repositories;
using AsyncApi.Worker.Repositories.DAL;
using AsyncApi.Worker.Repositories.Interfaces;
using AsyncApi.Worker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AsyncApi.Worker
{
    public static class ServiceCollectionExtension
    {       
        public static void AddServices(this IServiceCollection services, IConfigurationRoot configuration)
        {
            var connectionString = configuration.GetConnectionString("TaskDBConnectionString");

            var dbOptions = new DbContextOptionsBuilder<TaskDBContext>();
            dbOptions.UseSqlServer(connectionString);
            services.AddSingleton<ITaskDBContextFactory>(t => new TaskDBContextFactory(dbOptions));
            services.AddSingleton<ITaskDBContext, TaskDBContext>();
            services.AddSingleton<ITaskRepository, TaskRepository>();
            services.AddSingleton<ITaskService, TaskService>();
            services.AddSingleton<IClient, Client>();
            services.AddSingleton<IClientFactory, ClientFactory>();
            services.AddSingleton<IMessageProcessor, CreateClientMessageProcessor>();
            services.AddSingleton<IConsumerFactory, ConsumerFactory>();

            var callBackServerConfiguration = configuration.GetSection("CallBackServer").Get<SenderConfiguration>();
            services.AddSingleton<ISender>(t => new Sender(new ConnectionFactory
            {
                HostName = callBackServerConfiguration.Server.Host,
                UserName = callBackServerConfiguration.Server.UserName,
                Password = callBackServerConfiguration.Server.Password
            },
            callBackServerConfiguration.ExchangeName));

            var serviceProvider = services.BuildServiceProvider();
            var apiConfigurations = configuration.GetSection("APIConfigurations").Get<IEnumerable<ClientConfiguration>>();
            services.AddSingleton<ICustomerService>(t => new CustomerService(
                serviceProvider.GetService<IClientFactory>(), 
                apiConfigurations.FirstOrDefault(c => c.Client == Clients.Customers), 
                serviceProvider.GetService<ILogger<ICustomerService>>())
            );
        }

        public static void AddListeners(this IServiceCollection services, ServiceProvider serviceProvider, IConfigurationRoot configuration)
        {
            var listeners = configuration.GetSection("Listeners").Get<IEnumerable<SenderConfiguration>>();

            if (listeners != null)
            {
                foreach (var listener in listeners)
                {
                    services.AddSingleton<IListener>(t => new Listener(new ConnectionFactory
                    {
                        HostName = listener.Server.Host,
                        UserName = listener.Server.UserName,
                        Password = listener.Server.Password
                    },
                    serviceProvider.GetService<IConsumerFactory>(),
                    listener.Type,
                    listener.QueueName,
                    listener.ExchangeName,
                    serviceProvider.GetServices<IMessageProcessor>(),
                    serviceProvider.GetService<ILogger<Listener>>()));
                }
            }
        }
    }
}
