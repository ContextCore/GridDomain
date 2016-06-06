using System;
using Akka.Actor;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Akka.Tasks;
using NLog;

namespace GridDomain.Scheduling.Akka
{
    public abstract class ScheduledMessageHandler<TRequest> : ReceiveActor
        where TRequest : ScheduledMessage
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        protected ScheduledMessageHandler()
        {
            Receive<TRequest>(request =>
            {
                try
                {
                    Handle(request);
                    Sender.Tell(new MessageSuccessfullyProcessed(request.TaskId));
                }
                catch (Exception e)
                {
                    Sender.Tell(new MessageProcessingFailed(request.TaskId, e));
                }
            });
        }

        protected override void Unhandled(object message)
        {
            //TODO::VZ:: is it called automatically when unsupported message comes?
            _log.Error($"Message {message} is unhandled by {this}");
        }

        protected abstract void Handle(TRequest request);
    }
}