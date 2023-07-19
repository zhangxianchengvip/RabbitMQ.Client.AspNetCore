using Auto.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client.AspNetCore.RabbitMQ.Client;
using System;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace RabbitMQ.Client.AspNetCore
{
    public static class RabbitMQClientAspNetCoreServiceCollectionExtension
    {
        public static IServiceCollection AddRabbitClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoOptions(configuration);

            configuration.GetSection(nameof(RabbitMQOptions)).Bind(Appsettings.RabbitMQOptions);

            services.AddSingleton<IConnectionFactory>(sp =>
            {
                var options = configuration.GetOptions<RabbitMQOptions>();
                return new ConnectionFactory()
                {
                    HostName = options.HostName,
                    Port = options.Port,
                    Password = options.Password,
                    UserName = options.UserName,
                    VirtualHost = options.VirtualHost,
                    DispatchConsumersAsync = true
                };
            });

            services.AddSingleton<IRabbitMQPersistentConnection, DefaultRabbitMQPersistentConnection>();

            services.AddSingleton(sp =>
            {
                var connection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                return connection.CreateModel();

            });

            return services;
        }

        public static IServiceCollection AddRabbitClient(this IServiceCollection services, Action<RabbitMQOptions> action)
        {
            return services;
        }
    }
}
