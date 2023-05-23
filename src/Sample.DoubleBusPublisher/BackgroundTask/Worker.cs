using MassTransit;
using Microsoft.Extensions.Hosting;
using Sample.DoubleBusPublisher.Interface;
using Sample.DoubleBusPublisher.Model;

namespace Sample.DoubleBusPublisher.BackgroundTask
{
    public class Worker : BackgroundService
    {
        private readonly IBus _bus;
        private readonly IMsgBus _msgBus;
        private readonly DataPublishConfig _dataConfig;
        private readonly MsgPublishConfig _msgConfig;

        public Worker(IBus bus, IMsgBus msgBus, DataPublishConfig dataConfig, MsgPublishConfig msgConfig)
        {
            _bus = bus;
            _msgBus = msgBus;
            _dataConfig = dataConfig;
            _msgConfig = msgConfig;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _bus.Publish<MetaStringModel>(new MetaStringModel
                {
                    Text = "Hello from DataBus"
                }, c =>
                {
                    c.SetRoutingKey(_dataConfig.RoutingKey);
                }, stoppingToken);

                await _msgBus.Publish<MetaGeneralModel>(new MetaGeneralModel
                {
                    meta = new MetaBaseModel
                    {
                        type = "Hello from MsgBus"
                    }
                }, c =>
                {
                    c.SetRoutingKey(_msgConfig.RoutingKey);
                }, stoppingToken);

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
