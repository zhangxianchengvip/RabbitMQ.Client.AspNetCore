using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;

namespace Sample.Controllers;
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IModel _channel;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IModel channel)
    {
        _logger = logger;
        _channel = channel;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {

        _channel.QueueDeclare(queue: "hello",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

        const string message = "Hello World!";
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: string.Empty,
                             routingKey: "hello",
                             basicProperties: null,
                             body: body);

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
