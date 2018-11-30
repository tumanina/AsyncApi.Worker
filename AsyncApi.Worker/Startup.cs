using System;
using System.Collections.Generic;
using AsyncApi.Worker.Configuration;
using AsyncApi.Worker.MessageBroker;
using AsyncApi.Worker.Repositories;
using AsyncApi.Worker.Repositories.DAL;
using AsyncApi.Worker.Repositories.Interfaces;
using AsyncApi.Worker.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using RabbitMQ.Client;

namespace AsyncApi.Worker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var connectionString = Configuration.GetConnectionString("TaskDBConnectionString");

            services.AddDbContext<TaskDBContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Singleton);

            services.AddSingleton<ITaskDBContext, TaskDBContext>();
            services.AddSingleton<ITaskRepository, TaskRepository>();
            services.AddSingleton<ITaskService, TaskService>();
            services.AddSingleton<IMessageProcessor, CreateClientMessageProcessor>();

            var configuration = Configuration.GetSection("CallBackServer").Get<SenderConfiguration>();
            services.AddSingleton<ISender>(t => new Sender(new ConnectionFactory
            {
                HostName = configuration.Server.Host,
                UserName = configuration.Server.UserName,
                Password = configuration.Server.Password
            },
            configuration.ExchangeName));

            var listeners = Configuration.GetSection("Listeners").Get<IEnumerable<SenderConfiguration>>();

            var serviceProvider = services.BuildServiceProvider();

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

            services.AddLogging((builder) => builder.SetMinimumLevel(LogLevel.Warning));
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            NLog.LogManager.LoadConfiguration("nlog.config");
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            RunListeners(serviceProvider);
        }

        private void RunListeners(IServiceProvider serviceProvider)
        {
            foreach (var listener in serviceProvider.GetServices<IListener>())
            {
                listener.Run();
            }
        }
    }
}
