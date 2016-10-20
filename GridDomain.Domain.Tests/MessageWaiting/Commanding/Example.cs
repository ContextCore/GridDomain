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
            var cmd = new SampleCommand();
            IMessageWaiterFactory node = null;

            var commandWaiter = node.NewCommandWaiter()
                                    .Expect<SampleEventA>()
                                       .And<SampleEventB>()
                                    .Create(TimeSpan.FromSeconds(10))
                                .Execute(cmd);

            var messageWaiter = node.NewWaiter()
                                        .Expect<SampleEventA>()
                                        .And<SampleEventB>()
                                    .Create(TimeSpan.FromSeconds(10));
        }
    }
}