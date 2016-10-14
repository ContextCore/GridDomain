using System;
using System.Linq;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{


    public class Example
    {
        class SampleCommand : ICommand
        {
            public Guid Id { get; }
            public Guid SagaId { get; }
        }

        class SampleEventA : ICommand
        {
            public Guid Id { get; }
            public Guid SagaId { get; }
        }

        class SampleEventB : ICommand
        {
            public Guid Id { get; }
            public Guid SagaId { get; }
        }

        public void Main()
        {
            IMessageWaiterProducer node = new AkkaMessagesWaiterBuilder(null,null,TimeSpan.FromSeconds(1000),null);

            var cmd = new SampleCommand();
            
            var observer = node.Expect()
                                  .Message<SampleEventA>(e => e.Id == cmd.Id)
                                  .Message<SampleEventA>(e => e.Id == cmd.Id)
                                  .Fault<SampleEventB>(f => f.Message.Id == cmd.Id)
                               .Create();

            var observerB = node.ExpectCommand()
                                  .Message<SampleEventA>(e => e.Id == cmd.Id)
                                  .Message<SampleEventA>(e => e.Id == cmd.Id)
                                  .Fault<SampleEventB>(f => f.Message.Id == cmd.Id)
                                .Create()
                                .Execute(cmd);

            var results = observer.ReceiveAll().Result;

            var result = results.Message<SampleEventA>();
            var resultA = results.All.OfType<SampleEventB>();
            var a = new object();

        }
    }
}