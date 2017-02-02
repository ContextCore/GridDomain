using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Node.Actors;

namespace GridDomain.Tests.XUnit.CommandPipe
{
    class EchoSleepActor : ReceiveActor
    {

        public EchoSleepActor(TimeSpan sleepTime, IActorRef watcher)
        {
            Receive<IMessageMetadataEnvelop>(m =>
                Task.Delay(sleepTime)
                    .ContinueWith(t => new HandlerExecuted(m))
                    .PipeTo(Self, Sender));

            Receive<HandlerExecuted>(m =>
            {
                watcher.Tell(m);
                Sender.Tell(m);
            });
        }
    }
}