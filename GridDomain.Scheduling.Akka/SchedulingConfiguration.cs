using GridDomain.Common;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Configuration;
using GridDomain.Scheduling.Quartz.Logging;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Scheduling.WebUI;
using Microsoft.Practices.Unity;
using Quartz.Spi;
using Serilog;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Scheduling
{
    public class SchedulingConfiguration
    {
        private readonly IQuartzConfig _quartzConfig;
        private readonly ILogger _logger;
        private readonly IPublisher _publisher;
        private readonly ICommandExecutor _commandExecutor;

        public SchedulingConfiguration(IQuartzConfig quartzConfig, ILogger logger, IPublisher publisher, ICommandExecutor executor)
        {
            _commandExecutor = executor;
            _publisher = publisher;
            _logger = logger;
            _quartzConfig = quartzConfig;
        }

        public void Register(IUnityContainer container)
        {
            container.RegisterType<QuartzSchedulerFactory>();

            //hard initialization to get named instance of IScheduler
            //Quartz keeps static list of all schedulers so we need to be sure 
            //our current scheduler is created from scratch with specified name

            container.RegisterType<IScheduler>(new ContainerControlledLifetimeManager(),
                                               new InjectionFactory(x =>
                                                                    {
                                                                        var factory = x.Resolve<QuartzSchedulerFactory>();
                                                                        factory.Initialize(_quartzConfig.Settings);
                                                                        var scheduler = factory.GetScheduler();
                                                                        scheduler.Start();
                                                                        return scheduler;
                                                                    }));

            container.RegisterType<IJobFactory, JobFactory>();
            container.RegisterType<QuartzJob>();
            container.RegisterInstance(_quartzConfig);
            container.RegisterInstance(_commandExecutor);
            container.RegisterInstance(_logger);
            container.RegisterInstance(_publisher);

            container.RegisterInstance<IRetrySettings>(_quartzConfig.RetryOptions);
            container.RegisterType<IRetryStrategy, ExponentialBackoffRetryStrategy>();

            container.RegisterType<IWebUiConfig, WebUiConfig>();
            container.RegisterType<IWebUiWrapper, WebUiWrapper>();
        }
    }
}