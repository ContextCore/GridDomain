using System;
using Autofac;
using Quartz;
using Quartz.Spi;

namespace GridDomain.Scheduling.Akka
{
    public class JobFactory : IJobFactory
    {
        private readonly IContainer _container;

        public JobFactory(IContainer container)
        {
            _container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return (IJob) _container.Resolve(bundle.JobDetail.JobType);
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