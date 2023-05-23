using Autofac;
using Autofac.Extensions.DependencyInjection;
using MassTransit;
using Sample.RabbitMQPublisher.BackgroundTask;
using Sample.RabbitMQPublisher.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
	})
	.ConfigureServices((hostContext, service) =>
	{
		service.AddMassTransit(cfg =>
		{
			var setting = hostContext.Configuration.GetSection(nameof(RabbitMqConfig)).GetSection(nameof(DataPublishConfig)).Get<DataPublishConfig>();
			cfg.UsingRabbitMq((context, cfg) =>
			{
				cfg.Host(setting.Host, setting.VirtualHost, cfg =>
				{
					cfg.Username(setting.UserName);
					cfg.Password(setting.Password);
				});

				cfg.Message<MetaStringModel>(x => x.SetEntityName(setting.ExchangeName)) ;

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

        service.AddHostedService<Worker>();
	});
