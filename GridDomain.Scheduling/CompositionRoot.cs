using Akka.Actor;
using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Scheduling.Integration;
using GridDomain.Scheduling.Quartz;
using GridDomain.Scheduling.Quartz.Logging;
using GridDomain.Scheduling.WebUI;
using Microsoft.Practices.Unity;
using Quartz;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Scheduling
{
    public class CompositionRoot
    {
        public UnityContainer Compose(UnityContainer container, ActorSystem actorSystem)
        {
            container.RegisterType<IScheduler>(new InjectionFactory(x => x.Resolve<ISchedulerFactory>()));
            container.RegisterType<ISchedulerFactory, SchedulerFactory>();
            container.RegisterType<IScheduler>(new InjectionFactory(x => x.Resolve<ISchedulerFactory>().GetScheduler()));
            container.RegisterType<IQuartzConfig, QuartzConfig>();
            container.RegisterType<IQuartzLogger, QuartzLogger>();
            container.RegisterType<QuartzJob>();
            container.RegisterType<ActorSystem>(new InjectionFactory(x => actorSystem));
            container.RegisterType<ILoggingJobListener, LoggingJobListener>();
            container.RegisterType<ILoggingSchedulerListener, LoggingSchedulerListener>();

            var transport = new AkkaEventBusTransport(actorSystem);
            container.RegisterInstance<IPublisher>(transport);
            container.RegisterInstance<IActorSubscriber>(transport, new ContainerControlledLifetimeManager());
            container.RegisterType<IWebUiConfig, WebUiConfig>();
            container.RegisterType<IWebUiWrapper, WebUiWrapper>();
            return container;
        }
    }
}
