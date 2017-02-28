using System;
using Microsoft.Practices.Unity;
using Quartz;
using Quartz.Spi;

namespace GridDomain.Scheduling.Quartz
{
    public class JobFactory : IJobFactory
    {
        private readonly IUnityContainer _container;

        public JobFactory(IUnityContainer container)
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