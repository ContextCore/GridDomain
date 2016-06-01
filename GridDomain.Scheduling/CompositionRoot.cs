using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Logging;
using Microsoft.Practices.Unity;
using Quartz;
using Quartz.Unity;

namespace GridDomain.Scheduling
{
    public class CompositionRoot
    {
        public UnityContainer Compose(UnityContainer container)
        {
            container.AddNewExtension<QuartzUnityExtension>();

            container.RegisterType<IScheduler>(new InjectionFactory(x => x.Resolve<ISchedulerFactory>()));
            container.RegisterType<ISchedulerFactory, SchedulerFactory>();
            container.RegisterType<IScheduler>(new InjectionFactory(x => x.Resolve<ISchedulerFactory>().GetScheduler()));
            container.RegisterType<IQuartzConfig, QuartzConfig>();
            container.RegisterType<IQuartzLogger, QuartzLogger>();
            container.RegisterType<ITaskRouter, TaskRouter>();
            container.RegisterType<ILoggingJobListener, LoggingJobListener>();
            container.RegisterType<ILoggingSchedulerListener, LoggingSchedulerListener>();
            return container;
        }
    }
}
