using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Akka.Tasks;
using NLog;

namespace GridDomain.Scheduling.Akka
{
    public abstract class ScheduledTaskHandler<TRequest> : ReceiveActor
        where TRequest : ScheduledRequest
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        protected ScheduledTaskHandler()
        {
            //TODO::VZ:: use typed/untyped method, what about async handlers?
            Receive<TRequest>(request => HandleBase(request));
        }

        protected override void Unhandled(object message)
        {
            //TODO::VZ:: is it called automatically when unsupported message comes?
            _log.Error($"Message {message} is unhandled by {this}");
        }

        protected abstract Task Handle(TRequest request);

        private void HandleBase(TRequest request)
        {
            Handle(request).ContinueWith(task =>
            {
                //TODO::VZ:: when base method throws exception instead of returning task (concrete handling method isn`t marked as async), continuation doesn`t work
                if (task.IsFaulted && task.Exception != null)
                {
                    Sender.Tell(new Failure { Exception = task.Exception.InnerException });
                }
                else
                {
                    Sender.Tell(new TaskProcessed(request.TaskId));
                }
            }, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.ExecuteSynchronously)
            .PipeTo(Self);
        }
    }
}