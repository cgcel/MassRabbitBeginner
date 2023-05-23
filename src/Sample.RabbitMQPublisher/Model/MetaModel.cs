namespace Sample.RabbitMQPublisher.Model
{
    public class MetaBaseModel
    {
        public string type { get; set; }
        public string st { get; set; }
        public string sender { get; set; }
    }

    public class MetaGeneralModel
    {
        public MetaBaseModel meta { get; set; }
        public dynamic data { get; set; }
    }

    public class MetaStringModel
    {
        public string Text { get; set; }
    }
}
