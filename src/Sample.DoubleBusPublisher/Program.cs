using Autofac;
using Autofac.Extensions.DependencyInjection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.DoubleBusPublisher.BackgroundTask;
using Sample.DoubleBusPublisher.Interface;
using Sample.DoubleBusPublisher.Model;

try
{
    var app = CreateHostBuilder(args).Build();
    await app.RunAsync();
}
catch (Exception)
{

    throw;
}

IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>((hostContext, builder) =>
    {
        builder.Register(c =>
        {
            var setting = hostContext.Configuration.GetSection(nameof(RabbitMqConfig)).GetSection((nameof(DataPublishConfig))).Get<DataPublishConfig>();
            return setting;
        }).As<DataPublishConfig>();
        builder.Register(c =>
        {
            var setting = hostContext.Configuration.GetSection(nameof(RabbitMqConfig)).GetSection((nameof(MsgPublishConfig))).Get<MsgPublishConfig>();
            return setting;
        }).As<MsgPublishConfig>();
    })
    .ConfigureServices((hostContext, service) =>
    {
        service.AddMassTransit(cfg =>
        {
            var setting = hostContext.Configuration.GetSection(nameof(RabbitMqConfig)).GetSection(nameof(DataPublishConfig)).Get<DataPublishConfig>();
            cfg.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(setting.Host, ushort.Parse(setting.Port), setting.VirtualHost, cfg =>
                {
                    cfg.Username(setting.UserName);
                    cfg.Password(setting.Password);
                });

                cfg.UseRawJsonSerializer();

                cfg.Message<MetaStringModel>(x => x.SetEntityName(setting.ExchangeName));

                cfg.Publish<MetaStringModel>(e =>
                {
                    e.Durable = true;
                    e.ExchangeType = "topic";
                    e.AutoDelete = false;
                    e.BindQueue(setting.ExchangeName, setting.QueueName, c =>
                    {
                        c.Durable = true;
                        c.AutoDelete = false;
                        c.ExchangeType = "topic";
                        c.RoutingKey = setting.RoutingKey;
                        c.SetQueueArgument("x-message-ttl", 259200000);
                    });
                });

            });
        });

        service.AddMassTransit<IMsgBus>(cfg =>
        {
            var setting = hostContext.Configuration.GetSection(nameof(RabbitMqConfig)).GetSection(nameof(MsgPublishConfig)).Get<MsgPublishConfig>();
            cfg.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(setting.Host, ushort.Parse(setting.Port), setting.VirtualHost, cfg =>
                {
                    cfg.Username(setting.UserName);
                    cfg.Password(setting.Password);
                });

                cfg.UseRawJsonSerializer();

                cfg.Message<MetaGeneralModel>(x => x.SetEntityName(setting.ExchangeName));

                cfg.Publish<MetaGeneralModel>(e =>
                {
                    e.Durable = true;
                    e.ExchangeType = "topic";
                    e.AutoDelete = false;
                    e.BindQueue(setting.ExchangeName, setting.QueueName, c =>
                    {
                        c.Durable = true;
                        c.AutoDelete = false;
                        c.ExchangeType = "topic";
                        c.RoutingKey = setting.RoutingKey;
                        c.SetQueueArgument("x-message-ttl", 259200000);
                    });
                });

            });
        });

        service.AddHostedService<Worker>();
    });
