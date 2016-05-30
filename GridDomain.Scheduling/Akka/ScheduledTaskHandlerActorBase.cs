using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Akka.Tasks;
using NLog;

namespace GridDomain.Scheduling.Akka
{
    public abstract class ScheduledTaskHandlerActorBase<TRequest> : ActorBase
        where TRequest : ProcessScheduledTaskRequest
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        protected override bool Receive(object message)
        {
            //TODO::VZ:: better way without cast?
            var request = message as ProcessScheduledTaskRequest;
            if (request == null)
            {
                _log.Error($"Message {message} routed to handler {GetType().FullName}");
                return false;
            }
            //TODO::VZ:: better way without cast?
            HandleBase(message as TRequest).PipeTo(Self);
            return true;
        }

        protected abstract Task Handle(TRequest request);

        private async Task<IProcessingResult> HandleBase(TRequest request)
        {
            try
            {
                await Handle(request);
                return new TaskProcessed(request.TaskId);
            }
            catch (Exception e)
            {
                return new TaskProcessingFailed(request.TaskId, e);
            }
        }
    }
}