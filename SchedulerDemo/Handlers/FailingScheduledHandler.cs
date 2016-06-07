using System;
using GridDomain.Scheduling.Akka;
using SchedulerDemo.ScheduledMessages;

namespace SchedulerDemo.Handlers
{
    public class FailingScheduledHandler : ScheduledMessageHandler<FailScheduledMessage>
    {
        protected override void Handle(FailScheduledMessage request)
        {
            throw new InvalidOperationException();
        }
    }
}