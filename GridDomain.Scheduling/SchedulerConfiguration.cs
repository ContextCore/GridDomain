using System;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Logging;
using GridDomain.Scheduling.WebUI;
using Microsoft.Practices.Unity;
using Quartz;
using Quartz.Spi;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Scheduling 
{
    public class SchedulerConfiguration : IContainerConfiguration
    {
        private readonly IQuartzConfig _quartzConfig;

        public SchedulerConfiguration(IQuartzConfig quartzConfig)
        {
            _quartzConfig = quartzConfig;
        }

        public void Register(IUnityContainer container)
        {
            container.RegisterType<ISchedulerFactory, SchedulerFactory>(new ContainerControlledLifetimeManager());
            container.RegisterType<IScheduler>(new ContainerControlledLifetimeManager(),
                                               new InjectionFactory(x => x.Resolve<ISchedulerFactory>().GetScheduler()));

            container.RegisterInstance<IQuartzConfig>(_quartzConfig);
            container.RegisterType<IQuartzLogger, QuartzLogger>();
            container.RegisterType<IJobFactory, JobFactory>();
            container.RegisterType<QuartzJob>();
            
            container.RegisterType<ILoggingJobListener, LoggingJobListener>();
            container.RegisterType<ILoggingSchedulerListener, LoggingSchedulerListener>();

            container.RegisterType<IWebUiConfig, WebUiConfig>();
            container.RegisterType<IWebUiWrapper, WebUiWrapper>();

            //TODO: unify with GridDomain.Node.Configuration.Composition.SagaConfiguration

            var factory = new ScheduledCommandProcessingSagaFactory();
            var producer = new SagaProducer<ScheduledCommandProcessingSaga>(ScheduledCommandProcessingSaga.SagaDescriptor);
            producer.Register<ScheduledCommandProcessingStarted>(factory);
            producer.Register<ScheduledCommandProcessingSagaState>(factory);

            container.RegisterInstance<ISagaProducer<ScheduledCommandProcessingSaga>>(producer);
        }
    }
}
