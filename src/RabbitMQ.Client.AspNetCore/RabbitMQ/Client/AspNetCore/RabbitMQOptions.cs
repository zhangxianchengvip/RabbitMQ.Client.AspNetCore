using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitMQ.Client.AspNetCore
{
    public class RabbitMQOptions
    {
        public const string DefaultPass = "guest";

        public const string DefaultUser = "guest";

        public const string DefaultVHost = "/";

        public const string DefaultHost = "localhost";

        public const string DefaultExchangeName = "fsbus.default.exchange";

        public const int DefaultPort = 5672;

        public string HostName { get; set; } = DefaultHost;

        public string Password { get; set; } = DefaultPass;

        public string UserName { get; set; } = DefaultUser;

        public string VirtualHost { get; set; } = DefaultVHost;

        public string ExchangeName { get; set; } = DefaultExchangeName;

        public bool PublishConfirms { get; set; }

        public int Port { get; set; } = DefaultPort;

        public string ExchangeType { get; set; }
    }
}
