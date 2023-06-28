using MassTransit;
using Sample.MultiConsumer.Model;

namespace Sample.MultiConsumer.Service
{
    public class MsgConsumeService : IConsumer<MetaGeneralModel>
    {
        public Task Consume(ConsumeContext<MetaGeneralModel> context)
        {
            Console.WriteLine(context.Message.meta.type);
            return Task.CompletedTask;
        }
    }
}
