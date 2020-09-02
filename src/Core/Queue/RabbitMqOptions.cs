﻿namespace Queue
{
    public class RabbitMqOptions
    {
        public string Scheme { get; set; } = "rabbitmq";
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
    }
}