using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.Client.AspNetCore.RabbitMQ.Client
{
    public static class Appsettings
    {
        public static RabbitMQOptions RabbitMQOptions { get; set; } = new RabbitMQOptions();
    }
}
