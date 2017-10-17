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
    internal class TestProcessActor : ReceiveActor
    {
        public TestProcessActor(IActorRef watcher, Guid id,
                                TimeSpan? sleepTime = null)
        {
            var sleep = sleepTime ?? TimeSpan.FromMilliseconds(10);

            Receive<IMessageMetadataEnvelop<DomainEvent>>(
                                                          m =>
                                                          {
                                                              Task.Delay(sleep)
                                                                  .ContinueWith(t => 
                                                                  new ProcessTransited(new []{new TestCommand(m.Message.SourceId){ProcessId = id}}, 
                                                                                        m.Metadata, ProcessEntry.Empty,null))
                                                                  .PipeTo(Self, Sender);
                                                          });


            Receive<ProcessTransited>(m =>
                                   {
                                       watcher.Tell(m);
                                       Sender.Tell(m);
                                   });
        }
    }
}