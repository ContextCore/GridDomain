using System;
using Akka.Actor;
using Autofac;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using GridDomain.Scheduling.Quartz.Configuration;
using Serilog;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Scheduling.Akka {
    public static class SchedulingExtensions
    {
        public static SchedulingExtension InitSchedulingExtension(this ActorSystem system,
                                                                  IQuartzConfig quartzConfig,
                                                                  ILogger logger,
                                                                  IPublisher publisher,
                                                                  ICommandExecutor executor)
        {
            if(system == null)
                throw new ArgumentNullException(nameof(system));

            var ext = (SchedulingExtension)system.RegisterExtension(SchedulingExtensionProvider.Provider);
            var schedulingContainer = new ContainerBuilder();
            new SchedulingConfiguration(quartzConfig, logger, publisher, executor).Register(schedulingContainer);
            var container = schedulingContainer.Build();

            ext.Scheduler = container.Resolve<IScheduler>();
            ext.SchedulingActor = system.ActorOf(Props.Create(() => new SchedulingActor()), nameof(SchedulingActor));

            system.RegisterOnTermination(() =>
                                         {
                                             try
                                             {
                                                 if(ext.Scheduler != null && ext.Scheduler.IsShutdown == false)
                                                     ext.Scheduler.Shutdown(false);
                                             }
                                             catch(Exception ex)
                                             {
                                                 system.Log.Warning($"Got error on quartz scheduler shutdown:{ex}");
                                             }
                                         });
            return ext;
        }
    }
}