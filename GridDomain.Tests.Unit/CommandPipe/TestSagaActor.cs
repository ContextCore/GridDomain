using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.Messages;

namespace GridDomain.Tests.Unit.CommandPipe
{
    internal class TestSagaActor : ReceiveActor
    {
        public TestSagaActor(IActorRef watcher,
                             Func<DomainEvent, ICommand[]> commandFactory = null,
                             TimeSpan? sleepTime = null)
        {
            var sleep = sleepTime ?? TimeSpan.FromMilliseconds(10);
            commandFactory = commandFactory ?? (e => new ICommand[] {new TestCommand(e.SourceId)});

            Receive<IMessageMetadataEnvelop<DomainEvent>>(
                                                          m =>
                                                          {
                                                              Task.Delay(sleep)
                                                                  .ContinueWith(t => new SagaTransited(commandFactory(m.Message), m.Metadata, ProcessEntry.Empty,null))
                                                                  .PipeTo(Self, Sender);
                                                          });


            Receive<SagaTransited>(m =>
                                   {
                                       watcher.Tell(m);
                                       Sender.Tell(m);
                                   });
        }
    }
}