using MassTransit;
using Sample.RabbitMQPublisher.Model;
using Microsoft.Extensions.Hosting;

namespace Sample.RabbitMQPublisher.BackgroundTask
{
    public class Worker : BackgroundService
    {
        private readonly IBus _bus;
        private readonly DataPublishConfig _config;

        public Worker(IBus bus, DataPublishConfig config)
        {
            _bus = bus;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _bus.Publish<MetaStringModel>(new MetaStringModel
                {
                    Text = "Hello World"
                }, c =>
                {
                    c.SetRoutingKey(_config.RoutingKey);
                }, stoppingToken);

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
