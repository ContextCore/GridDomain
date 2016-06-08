using System;
using System.Threading;
using GridDomain.Scheduling.Akka;
using SchedulerDemo.ScheduledMessages;

namespace SchedulerDemo.Handlers
{
    public class LongTimeScheduledHandler : ScheduledMessageHandler<LongTimeScheduledCommand>
    {
        protected override void Handle(LongTimeScheduledCommand request)
        {
            Thread.Sleep(TimeSpan.FromSeconds(request.SecondsToExecute));
        }
    }
}