using System;
using GridDomain.CQRS.Messaging;
using GridDomain.Scheduling.Quartz.Logging;
using SchedulerDemo.Messages;

namespace SchedulerDemo
{
    public class DemoLogger : IQuartzLogger
    {
        private readonly IPublisher _publisher;

        public DemoLogger(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void LogSuccess(string jobName)
        {
            _publisher.Publish(new WriteToConsole($"{jobName} successfully finished"));
        }

        public void LogFailure(string jobName, Exception e)
        {
            _publisher.Publish(new WriteErrorToConsole(e));
        }
    }
}