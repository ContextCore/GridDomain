using System;
using Akka.Actor;
using Akka.Event;
using Autofac.Core.Activators;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Node.Actors.EventSourced.Messages;

namespace GridDomain.Node.Actors.RecycleMonitor
{
    public class RecycleMonitorActor:ReceiveActor
    {
        public RecycleMonitorActor(IRecycleConfiguration recycleConfiguration, IActorRef watched)
        {
            Context.Watch(watched);
            var lastActivityTime = BusinessDateTime.UtcNow;
            var log = Context.GetLogger();
            var schedule = ScheduleCheck(recycleConfiguration.ChildClearPeriod);
            var isWaitingForShutdown = false;
            lastActivityTime = BusinessDateTime.UtcNow;;

            Receive<Activity>(a =>
                              {
                                  var now = BusinessDateTime.UtcNow;
                                  var inactiveTime = now - lastActivityTime;
                                  lastActivityTime = now;
                                  log.Debug($"Received activity after {inactiveTime}");
                                  if (!isWaitingForShutdown) return;
                                  log.Debug($"Rescheduling schecks after receiving activity after shutdown request");

                                  isWaitingForShutdown = false;
                                  schedule = ScheduleCheck(recycleConfiguration.ChildClearPeriod);
                              }, a => Sender.Path == watched.Path);
            Receive<Check>(c =>
                           {                                                                           
                               var inactivityDuration = BusinessDateTime.UtcNow - lastActivityTime;
                               log.Debug($"inactivity duration: {inactivityDuration}");
                               if (inactivityDuration > recycleConfiguration.ChildMaxInactiveTime)
                               {
                                   log.Debug($"Sending graceful shutdown request to {watched} due to inactivity for {inactivityDuration} with max allowed {recycleConfiguration.ChildMaxInactiveTime}");
                                   watched.Tell(GracefullShutdownRequest.Instance);
                                   isWaitingForShutdown = true;
                               }
                               schedule = ScheduleCheck(recycleConfiguration.ChildClearPeriod);
                           });
            Receive<Terminated>(t =>
                                {
                                    schedule.Cancel();
                                    Context.Stop(Self);
                                }, t => t.ActorRef.Path == watched.Path);
        }

        private ICancelable ScheduleCheck(TimeSpan childClearPeriod)
        {
            return Context.System.Scheduler.ScheduleTellOnceCancelable(childClearPeriod,
                                                                       Self,
                                                                       Check.Instance,
                                                                       Self);
        }

        public class Check
        {
            public static Check Instance = new Check();
            private  Check(){}
        }

        public class Activity
        {
            public static Activity Instance = new Activity();
            private  Activity(){}
        }
    }
}