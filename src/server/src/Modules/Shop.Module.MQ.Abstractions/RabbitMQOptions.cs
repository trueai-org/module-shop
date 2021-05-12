namespace Shop.Module.MQ
{
    public class RabbitMQOptions
    {
        public bool Enabled { get; set; }

        public string Host { get; set; }

        public ushort Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
