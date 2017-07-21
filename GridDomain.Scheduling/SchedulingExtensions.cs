using System;
using Akka.Actor;
using GridDomain.Configuration;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using GridDomain.Scheduling.Akka;
using GridDomain.Scheduling.Quartz.Configuration;
using Microsoft.Practices.Unity;
using Serilog;
using IScheduler = Quartz.IScheduler;

namespace GridDomain.Scheduling {
    public static class SchedulingExtensions
    {
        public static SchedulingExtension InitSchedulingExtension(this ActorSystem system,
                                                                  IQuartzConfig eventAdapters,
                                                                  ILogger logger,
                                                                  IPublisher publisher,
                                                                  ICommandExecutor executor)
        {
            if(system == null)
                throw new ArgumentNullException(nameof(system));

            var ext = (SchedulingExtension)system.RegisterExtension(SchedulingExtensionProvider.Provider);
            var schedulingContainer = new UnityContainer();
            schedulingContainer.Register(new SchedulingConfiguration(eventAdapters, logger, publisher, executor));

            ext.Scheduler = schedulingContainer.Resolve<IScheduler>();
            ext.SchedulingActor = system.ActorOf(Props.Create(() => new SchedulingActor(ext.Scheduler, publisher)), nameof(SchedulingActor));

            system.RegisterOnTermination(() =>
                                         {
                                             try
                                             {
                                                 if(ext.Scheduler != null && ext.Scheduler.IsShutdown == false)
                                                     ext.Scheduler.Shutdown();
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