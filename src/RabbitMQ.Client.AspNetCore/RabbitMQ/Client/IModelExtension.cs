using Polly;
using Polly.Retry;
using RabbitMQ.Client.AspNetCore;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace RabbitMQ.Client
{
    public static class IModelExtension
    {
        public static void BasicPublish(this IModel channel, byte[] body, string exchangeType, string exchange = null, string routingKey = null)
        {
            if (channel == null)
                throw new ArgumentNullException(nameof(channel));


            var policy = RetryPolicy
                .Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(retryCount: 5, sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                 onRetry: (ex, time) =>
                 {
                     Console.WriteLine(ex.Message);
                 });


            channel.ExchangeDeclare(exchange: exchange, type: exchangeType);

            policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // persistent
                //properties.Priority = priority;

                channel.BasicPublish(
                                    exchange: "",
                                    routingKey: routingKey,
                                    mandatory: true,
                                    basicProperties: properties,
                                    body: body);
            });
        }
    }
}
