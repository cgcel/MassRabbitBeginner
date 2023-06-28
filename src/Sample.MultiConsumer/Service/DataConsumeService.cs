using MassTransit;
using Sample.MultiConsumer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.MultiConsumer.Service
{
    public class DataConsumeService : IConsumer<MetaStringModel>
    {
        public Task Consume(ConsumeContext<MetaStringModel> context)
        {
            Console.WriteLine(context.Message.Text);
            return Task.CompletedTask;
        }
    }
}
