using MassTransit;
using Sample.SimpleConsumer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.SimpleConsumer.Service
{
    public class ConsumeService : IConsumer<MetaStringModel>
    {
        public Task Consume(ConsumeContext<MetaStringModel> context)
        {
            Console.WriteLine(context.Message.Text);
            return Task.CompletedTask;
        }
    }
}
