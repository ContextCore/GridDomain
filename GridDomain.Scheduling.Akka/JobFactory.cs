using System;
using Autofac;
using Quartz;
using Quartz.Spi;

namespace GridDomain.Scheduling.Akka
{
    public class JobFactory : IJobFactory
    {
        private readonly ILifetimeScope _scope;

        public JobFactory(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return (IJob) _scope.Resolve(bundle.JobDetail.JobType);
        }

        public void ReturnJob(IJob job)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            // with direct cast it is possible to have an exception
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}