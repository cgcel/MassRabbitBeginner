using Autofac;
using Autofac.Extensions.DependencyInjection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Sample.MultiConsumer.Interface;
using Sample.MultiConsumer.Model;
using Sample.MultiConsumer.Service;

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
            var setting = hostContext.Configuration.GetSection(nameof(RabbitMqConfig)).GetSection((nameof(DataSubscribeConfig))).Get<DataSubscribeConfig>();
            return setting;
        }).As<DataSubscribeConfig>();
    })
    .ConfigureServices((hostContext, service) =>
    {
        service.AddMassTransit(cfg =>
        {
            var setting = hostContext.Configuration.GetSection(nameof(RabbitMqConfig)).GetSection(nameof(DataSubscribeConfig)).Get<DataSubscribeConfig>();
            cfg.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(setting.Host, setting.VirtualHost, cfg =>
                {
                    cfg.Username(setting.UserName);
                    cfg.Password(setting.Password);
                });

                cfg.ReceiveEndpoint(setting.QueueName, e =>
                {
                    e.ConfigureConsumeTopology = false;

                    e.SetQueueArgument("x-message-ttl", 259200000);

                    e.PrefetchCount = 50;

                    e.Consumer<DataConsumeService>();

                    e.UseRawJsonDeserializer();

                });

            });
        });

        service.AddMassTransit<IMsgBus>(cfg =>
        {
            var setting = hostContext.Configuration.GetSection(nameof(RabbitMqConfig)).GetSection(nameof(MsgSubscribeConfig)).Get<MsgSubscribeConfig>();
            cfg.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(setting.Host, setting.VirtualHost, cfg =>
                {
                    cfg.Username(setting.UserName);
                    cfg.Password(setting.Password);
                });

                cfg.ReceiveEndpoint(setting.QueueName, e =>
                {
                    e.ConfigureConsumeTopology = false;

                    e.SetQueueArgument("x-message-ttl", 259200000);

                    e.PrefetchCount = 50;

                    e.Consumer<MsgConsumeService>();

                    e.UseRawJsonDeserializer();

                });

            });
        });

    });
