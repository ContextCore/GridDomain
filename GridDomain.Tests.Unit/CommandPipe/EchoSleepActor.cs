using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Node.Actors;

namespace GridDomain.Tests.Unit.CommandPipe
{
    internal class EchoSleepActor : ReceiveActor
    {
        private string _name;

        public EchoSleepActor(TimeSpan sleepTime, IActorRef watcher)
        {
            _name = Self.Path.Name;
            Receive<IMessageMetadataEnvelop>(m => Task.Delay(sleepTime)
                                                      .ContinueWith(t => new MarkedHandlerExecutedMessage(_name, m))
                                                      .PipeTo(Self,Sender));

            Receive<MarkedHandlerExecutedMessage>(m =>
                                                  {
                                                      watcher.Tell(m);
                                                      Sender.Tell(m);
                                                  });
        }
    }
}