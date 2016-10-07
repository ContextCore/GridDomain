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
            IMessageWaiterProducer node = new AkkaMessageWaiterBuilder();

            var cmd = new SampleCommand();
            
            var observer = node.NewWaiter()
                                  .Message<SampleEventA>(e => e.Id == cmd.Id)
                                  .Message<SampleEventA>(e => e.Id == cmd.Id)
                                  .Fault<SampleEventB>(f => f.Message.Id == cmd.Id)
                               .Create();

            var observerB = node.NewCommandWaiter()
                                  .Message<SampleEventA>(e => e.Id == cmd.Id)
                                  .Message<SampleEventA>(e => e.Id == cmd.Id)
                                  .Fault<SampleEventB>(f => f.Message.Id == cmd.Id)
                                  .Create()
                                .Execute(cmd);

            observer.WaitAll();

            var result = observer.Received<SampleEventA>();
            var resultA = observer.AllReceivedMessages.OfType<SampleEventB>();
            var a = new object();

        }
    }
}