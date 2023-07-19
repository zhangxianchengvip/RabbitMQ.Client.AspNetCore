using Polly;
using Polly.Retry;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace RabbitMQ.Client.AspNetCore.RabbitMQ.Client
{
    public static class IModelExtension
    {
        public static void BasicPublish(this IModel channel, string topic, byte[] body)
        {
            if (channel == null)
                throw new ArgumentNullException(nameof(channel));

            var options = Appsettings.RabbitMQOptions;

            var policy = RetryPolicy
                .Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(retryCount: 5, sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                 onRetry: (ex, time) =>
                 {
                    Console.WriteLine(ex.Message);
                 });


            channel.ExchangeDeclare(exchange: options.ExchangeName, type: options.ExchangeType);

            policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // persistent
                //properties.Priority = priority;

                channel.BasicPublish(
                                    exchange: options.ExchangeName,
                                    routingKey: topic,
                                    mandatory: true,
                                    basicProperties: properties,
                                    body: body);
            });
        }
    }
}
