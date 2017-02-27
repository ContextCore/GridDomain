using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Event;
using Akka.Monitoring;
using GridDomain.Common;
using GridDomain.Logging;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors
{
    /// <summary>
    /// Any child should be terminated by ShutdownRequest message
    /// </summary>
    public abstract class PersistentHubActor : ReceiveActor,
                                               IWithUnboundedStash
    {
        internal readonly IDictionary<Guid, ChildInfo> Children = new Dictionary<Guid, ChildInfo>();
        private readonly IPersistentChildsRecycleConfiguration _recycleConfiguration;
        //TODO: replace with more efficient implementation
        internal virtual TimeSpan ChildClearPeriod => _recycleConfiguration.ChildClearPeriod;
        internal virtual TimeSpan ChildMaxInactiveTime => _recycleConfiguration.ChildMaxInactiveTime;
        private readonly ActorMonitor _monitor;
        private readonly ILoggingAdapter Logger = Context.GetLogger();
        private readonly ProcessEntry _forwardEntry;
        public IStash Stash { get; set; }

        protected abstract string GetChildActorName(object message);
        protected abstract Guid GetChildActorId(object message);
        protected abstract Type GetChildActorType(object message);

        protected PersistentHubActor(IPersistentChildsRecycleConfiguration recycleConfiguration, string counterName)
        {
            _recycleConfiguration = recycleConfiguration;
            _monitor = new ActorMonitor(Context, $"Hub_{counterName}");
            _forwardEntry = new ProcessEntry(Self.Path.Name, "Forwarding to child", "All messages should be forwarded");

            Receive<Terminated>(t =>
                                {
                                    Guid id;
                                    if (!AggregateActorName.TryParseId(t.ActorRef.Path.Name, out id)) return;
                                    Children.Remove(id);
                                    //continue to process any remaining messages
                                    //for example when we are trying to resume terminating child with no success
                                    Stash.UnstashAll();
                                });
            Receive<ClearChildren>(m => Clear());
            Receive<ShutdownChild>(m => ShutdownChild(m.ChildId));
            Receive<ShutdownCanceled>(m =>
                                      {
                                          
                                          Guid id;
                                          if (!AggregateActorName.TryParseId(Sender.Path.Name, out id)) return;
                                          //child was resumed from planned shutdown
                                          Children[id].Terminating = false;
                                          Stash.UnstashAll();
                                          Logger.Debug("Child {id} resumed. Stashed messages will be sent to it", id);
                                      });
            Receive<CheckHealth>(s => Sender.Tell(new HealthStatus(s.Payload)));
            Receive<IMessageMetadataEnvelop>(messageWitMetadata =>
                                             {
                                                 ChildInfo knownChild;

                                                 messageWitMetadata.Metadata.History.Add(_forwardEntry);

                                                 var childId = GetChildActorId(messageWitMetadata.Message);
                                                 var name = GetChildActorName(messageWitMetadata.Message);

                                                 bool childWasCreated = false;
                                                 if (!Children.TryGetValue(childId, out knownChild))
                                                 {
                                                     childWasCreated = true;
                                                     knownChild = CreateChild(messageWitMetadata, name);
                                                     Children[childId] = knownChild;
                                                     Context.Watch(knownChild.Ref);
                                                 }
                                                 else
                                                 {
                                                     //terminating a child is quite long operation due to snapshots saving
                                                     //it is cheaper to resume child than wait for it termination and create rom scratch
                                                     if (knownChild.Terminating)
                                                     {
                                                         Stash.Stash();
                                                         knownChild.Ref.Tell(CancelShutdownRequest.Instance);
                                                         Logger.Debug(
                                                             "Stashing message {msg} for child {id}. Waiting for child resume from termination",
                                                             messageWitMetadata,
                                                             childId);

                                                         return;
                                                     }
                                                 }

                                                 knownChild.LastTimeOfAccess = BusinessDateTime.UtcNow;
                                                 knownChild.ExpiresAt = knownChild.LastTimeOfAccess + ChildMaxInactiveTime;
                                                 SendMessageToChild(knownChild, messageWitMetadata);

                                                 Logger.Debug("Message {msg} sent to {isknown} child {id}",
                                                     messageWitMetadata,
                                                     childWasCreated ? "new" : "known",
                                                     childId);
                                             });
        }

        protected virtual void SendMessageToChild(ChildInfo knownChild, IMessageMetadataEnvelop message)
        {
            knownChild.Ref.Tell(message);
        }

        private void Clear()
        {
            var now = BusinessDateTime.UtcNow;
            var childsToTerminate = Children.Where(c => now > c.Value.ExpiresAt && !c.Value.Terminating)
                                            .Select(ch => ch.Key)
                                            .ToArray();

            foreach (var childId in childsToTerminate)
                ShutdownChild(childId);

            Logger.Debug("Clear childs process finished, removing {childsToTerminate} childs", childsToTerminate.Length);
        }

        private void ShutdownChild(Guid childId)
        {
            ChildInfo childInfo;
            if (!Children.TryGetValue(childId, out childInfo)) return;

            childInfo.Ref.Tell(GracefullShutdownRequest.Instance);
            childInfo.Terminating = true;
        }

        public class ClearChildren {}

        protected override bool AroundReceive(Receive receive, object message)
        {
            _monitor.IncrementMessagesReceived();
            return base.AroundReceive(receive, message);
        }

        protected ChildInfo CreateChild(IMessageMetadataEnvelop messageWitMetadata, string name)
        {
            var childActorType = GetChildActorType(messageWitMetadata.Message);
            var props = Context.DI()
                               .Props(childActorType);
            var childActorRef = Context.ActorOf(props, name);
            return new ChildInfo(childActorRef);
        }

        protected override void PreStart()
        {
            _monitor.IncrementActorStarted();
            Logger.Debug("{ActorHub} is going to start", Self.Path);
            Context.System.Scheduler.ScheduleTellRepeatedly(ChildClearPeriod,
                ChildClearPeriod,
                Self,
                new ClearChildren(),
                Self);
        }

        protected override void PostStop()
        {
            _monitor.IncrementActorStopped();
            Logger.Debug("{ActorHub} was stopped", Self.Path);
        }
    }
}