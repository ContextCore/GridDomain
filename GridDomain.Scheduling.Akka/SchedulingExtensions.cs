using System;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using GridDomain.Scheduling.Quartz.Configuration;
using Microsoft.Practices.Unity;
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
            var schedulingContainer = new UnityContainer();
            new SchedulingConfiguration(quartzConfig, logger, publisher, executor).Register(schedulingContainer);

            ext.Scheduler = schedulingContainer.Resolve<IScheduler>();
            ext.SchedulingActor = system.ActorOf(Props.Create(() => new SchedulingActor(ext.Scheduler, publisher)), nameof(SchedulingActor));

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