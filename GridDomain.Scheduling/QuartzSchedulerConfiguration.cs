using Akka.Actor;
using GridDomain.Common;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Logging;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Scheduling.WebUI;
using Microsoft.Practices.Unity;
using Quartz;
using Quartz.Spi;
using Serilog;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Scheduling 
{
    public class QuartzSchedulerConfiguration: IContainerConfiguration
    {
        private readonly IQuartzConfig _quartzConfig;

        public QuartzSchedulerConfiguration(IQuartzConfig quartzConfig)
        {
            _quartzConfig = quartzConfig;
        }

        public void Register(IUnityContainer container)
        {
            container.RegisterType<QuartzSchedulerFactory>();
            container.RegisterType<IScheduler>(new ContainerControlledLifetimeManager(),
                                               new InjectionFactory(x =>
                                                        { var factory = x.Resolve<QuartzSchedulerFactory>();
                                                          factory.Initialize(_quartzConfig.Settings);
                                                          factory.GetScheduler();
                                                          return factory.GetScheduler(_quartzConfig.Name);
                                                        }
                                                        ));

            container.RegisterType<IQuartzLogger, QuartzLogger>();
            container.RegisterType<IJobFactory, JobFactory>();
            container.RegisterType<QuartzJob>();
            container.RegisterInstance<IQuartzConfig>(_quartzConfig);

            container.RegisterInstance<IRetrySettings>(new InMemoryRetrySettings());
            container.RegisterType<IRetryStrategy, ExponentialBackoffRetryStrategy>();
            
            container.RegisterType<IWebUiConfig,  WebUiConfig>();
            container.RegisterType<IWebUiWrapper, WebUiWrapper>();
        }
    }
}
