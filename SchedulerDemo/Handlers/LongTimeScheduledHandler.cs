using System;
using System.Threading;
using GridDomain.Scheduling.Akka;
using SchedulerDemo.ScheduledMessages;

namespace SchedulerDemo.Handlers
{
    public class LongTimeScheduledHandler : ScheduledMessageHandler<LongTimeScheduledMessage>
    {
        protected override void Handle(LongTimeScheduledMessage request)
        {
            Thread.Sleep(TimeSpan.FromSeconds(request.SecondsToExecute));
        }
    }
}