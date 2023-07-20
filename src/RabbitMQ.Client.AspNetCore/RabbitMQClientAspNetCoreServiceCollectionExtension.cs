using Auto.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RabbitMQ.Client.AspNetCore
{
    public static class RabbitMQClientAspNetCoreServiceCollectionExtension
    {
        public static IServiceCollection AddRabbitMQClient(this IServiceCollection services)
        {

            services.AddSingleton<IConnectionFactory>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();

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
    }
}
