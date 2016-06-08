using System;
using GridDomain.Scheduling.Akka;
using SchedulerDemo.ScheduledMessages;

namespace SchedulerDemo.Handlers
{
    public class FailingScheduledHandler : ScheduledMessageHandler<FailScheduledCommand>
    {
        protected override void Handle(FailScheduledCommand request)
        {
            throw new InvalidOperationException();
        }
    }
}