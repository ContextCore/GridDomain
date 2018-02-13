using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Event;
using Akka.Pattern;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Node.Actors.EventSourced.Messages;
using GridDomain.Node.Actors.RecycleMonitor;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors.PersistentHub
{
    /// <summary>
    ///     Any child should be terminated by ShutdownRequest message
    /// </summary>
    public abstract class PersistentHubActor : ReceiveActor,
                                               IWithUnboundedStash
    {
        private readonly ActorMonitor _monitor;
        private readonly IRecycleConfiguration _recycleConfiguration;
        private readonly ILoggingAdapter Log = Context.GetSeriLogger();

        protected PersistentHubActor(IRecycleConfiguration recycleConfiguration, string counterName)
        {
            _recycleConfiguration = recycleConfiguration;
            _monitor = new ActorMonitor(Context, $"Hub_{counterName}");
            var forwardEntry = new ProcessEntry(Self.Path.Name, "Forwarding to child", "All messages should be forwarded");

            Receive<NotifyOnPersistenceEvents>(m => SendToChild(m,GetChildActorName(m.Id),Sender));
            Receive<ShutdownChild>(m => {
                                       var childActorName = GetChildActorName(m.ChildId);
                                       var child = Context.Child(childActorName);
                                       if (child == Nobody.Instance) return;
                                       CreateRecyleMonitor(child, ChildFastShutdownRecycleConfiguration.Instance,
                                                           $"Shutdown_RecycleMonitor_{childActorName}");
                                   });
            Receive<WarmUpChild>(m =>
                                 {
                                     var created = InitChild(GetChildActorName(m.Id), out var info);
                                     Sender.Tell(new WarmUpResult(info, created));
                                 });

            Receive<CheckHealth>(s => Sender.Tell(new HealthStatus(s.Payload)));
            Receive<IMessageMetadataEnvelop>(messageWithMetadata =>
                                             {
                                                 var childId = GetChildActorId(messageWithMetadata);
                                                 if (string.IsNullOrEmpty(childId))
                                                     throw new InvalidOperationException("child id is empty." + messageWithMetadata);
                                                 
                                                 var name = GetChildActorName(childId);
                                                 if (string.IsNullOrEmpty(childId))
                                                     throw new InvalidOperationException("child name is empty." + messageWithMetadata);

                                                 messageWithMetadata.Metadata.History.Add(forwardEntry);
                                                 SendToChild(messageWithMetadata, name, Sender);
                                             });
        }

        private static int _recycleMonitorCollisionCounter = 0;
        
        private bool InitChild(string name, out IActorRef childRef)
        {
            childRef = Context.Child(name);
            if (childRef != Nobody.Instance) return false;
            
            childRef = Context.ActorOf(Context.DI().Props(ChildActorType), name);
            
            
            var monitorName = $"RecycleMonitor_{name}";
            var existingMonitor = Context.Child(monitorName);
            if (existingMonitor != Nobody.Instance)
            {
                //in rare cases of recreating existing child its old monitor will not be terminated 
                //as it terminates after child
                //so we just pick up another name, to not bother will waiting for old monitor termination
                _recycleMonitorCollisionCounter++;
                monitorName = $"RecycleMonitor_{_recycleMonitorCollisionCounter}_{name}";
            }
                
            CreateRecyleMonitor(childRef, _recycleConfiguration,monitorName);
            return true;
        }

        private void CreateRecyleMonitor(IActorRef child, IRecycleConfiguration recycleConfiguration, string name, IActorRef watcher=null)
        {
            Context.ActorOf(Props.Create(() => new RecycleMonitorActor(recycleConfiguration, child, watcher)),name);
        }

        protected void SendToChild(object message, string name, IActorRef sender)
        {
            bool childWasCreated;
            childWasCreated = InitChild(name, out var knownChild);
            SendMessageToChild(knownChild, message, sender);
            LogMessageSentToChild(message, name, childWasCreated);
        }

        private void LogMessageSentToChild(object message, string childName, bool childWasCreated)
        {
            if (message is IMessageMetadataEnvelop env)
            {
                Log.Debug("Message {msg} with metadata sent to {isknown} child {id}",
                    env.Message,
                    childWasCreated ? "new" : "known",
                    childName);
            }
            else
                Log.Debug("Message {msg} sent to {isknown} child {id}",
                    message,
                    childWasCreated ? "new" : "known",
                    childName);
        }

     
        public IStash Stash { get; set; }
        internal abstract string GetChildActorName(string childId);
        protected abstract string GetChildActorId(IMessageMetadataEnvelop message);
        protected abstract Type ChildActorType { get; }

        protected virtual void SendMessageToChild(IActorRef knownChild, object message, IActorRef sender)
        {
            knownChild.Tell(message, sender);
        }

        class ChildFastShutdownRecycleConfiguration : IRecycleConfiguration
        {
            public TimeSpan ChildClearPeriod { get; } = TimeSpan.FromMilliseconds(500);
            public TimeSpan ChildMaxInactiveTime { get; }= TimeSpan.Zero;

            private ChildFastShutdownRecycleConfiguration(){}
            public static ChildFastShutdownRecycleConfiguration Instance { get; } = new ChildFastShutdownRecycleConfiguration();
        }

        protected override bool AroundReceive(Receive receive, object message)
        {
            _monitor.IncrementMessagesReceived();
            return base.AroundReceive(receive, message);
        }

        protected override void PreStart()
        {
            _monitor.IncrementActorStarted();
        }

        protected override void PostStop()
        {
            _monitor.IncrementActorStopped();
            Log.Debug("Stopped");
        }
    }

    
}