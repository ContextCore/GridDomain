using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.Hadlers;

namespace GridDomain.Tests.Unit.CommandPipe
{
    class MarkedHandlerExecutedMessage : HandlerExecuted
    {
        public string Mark { get; }
        public MarkedHandlerExecutedMessage(string mark, IMessageMetadataEnvelop processingMessage, Exception error = null) : base(processingMessage, error)
        {
            Mark = mark;
        }
    }

    internal class EchoSleepActor : ReceiveActor
    {
       
        public EchoSleepActor(TimeSpan sleepTime, IActorRef watcher)
        {
            ReceiveAsync<IMessageMetadataEnvelop>(m => Task.Delay(sleepTime).ContinueWith(t =>
                                                                                          {
                                                                                              var handlerExecuted = new MarkedHandlerExecutedMessage(Self.Path.ToString(),m);
                                                                                              watcher.Tell(handlerExecuted);
                                                                                              return handlerExecuted;
                                                                                          }).PipeTo(Sender));
        }
    }
}