using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.CommandPipe;

namespace GridDomain.Tests.Unit.CommandPipe
{
    class TestSagaActor : ReceiveActor
    {

        public TestSagaActor(IActorRef watcher,
            Func<DomainEvent, ICommand[]> commandFactory = null,
            TimeSpan? sleepTime = null)
        {
            var sleep = sleepTime ?? TimeSpan.FromMilliseconds(10);
            commandFactory = commandFactory ?? (e => new ICommand[] { new TestCommand(e) });

            Receive<IMessageMetadataEnvelop<DomainEvent>>(m =>
            {
                Task.Delay(sleep)
                    .ContinueWith(t => new SagaProcessCompleted(commandFactory(m.Message), m.Metadata))
                    .PipeTo(Self, Sender);
            });


            Receive<SagaProcessCompleted>(m =>
            {
                watcher.Tell(m);
                Sender.Tell(m);
            });
        }
    }
}