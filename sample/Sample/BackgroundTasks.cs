using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace Sample;

public class BackgroundTasks : BackgroundService
{

    private readonly IModel _channel;
    public BackgroundTasks(IModel channel)
    {
        _channel = channel;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
       // await Task.Run(() => Consumer());
    }

    private void Consumer()
    {
        _channel.QueueDeclare(queue: "hello",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += Consumer_Received;
        _channel.BasicConsume(queue: "hello",
                             autoAck: true,
                             consumer: consumer);
    }
    private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
    {
        try
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);
            await Console.Out.WriteLineAsync(message);
        }
        catch (Exception ex)
        {

        }

        // Even on exception we take the message off the queue.
        // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
        // For more information see: https://www.rabbitmq.com/dlx.html
        _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
    }

}
