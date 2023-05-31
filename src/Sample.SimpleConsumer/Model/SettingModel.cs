namespace Sample.SimpleConsumer.Model
{
    /// <summary>
    /// rabbitmq publishing config
    /// </summary>
    public class RabbitMqConfig
    {
        public DataSubscribeConfig DataSubscribeConfig { get; set; }
    }

    /// <summary>
    /// mq publish config
    /// </summary>
    public class SubscribeConfig
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public string ExchangeName { get; set; }
        public string RoutingKey { get; set; }
        public string QueueName { get; set; }
    }    

    public class DataSubscribeConfig : SubscribeConfig
    {

    }
}
