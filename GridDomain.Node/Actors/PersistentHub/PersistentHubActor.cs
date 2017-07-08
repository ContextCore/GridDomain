using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Event;
using GridDomain.Common;
using GridDomain.Node.Actors.EventSourced.Messages;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Actors.PersistentHub
{
    /// <summary>
    ///     Any child should be terminated by ShutdownRequest message
    /// </summary>
    public abstract class PersistentHubActor : ReceiveActor,
                                               IWithUnboundedStash
    {
        private readonly ProcessEntry _forwardEntry;
        private readonly ActorMonitor _monitor;
        private readonly IPersistentChildsRecycleConfiguration _recycleConfiguration;
        internal readonly IDictionary<Guid, ChildInfo> Children = new Dictionary<Guid, ChildInfo>();
        private readonly ILoggingAdapter Log = Context.GetLogger();

        protected PersistentHubActor(IPersistentChildsRecycleConfiguration recycleConfiguration, string counterName)
        {
            _recycleConfiguration = recycleConfiguration;
            _monitor = new ActorMonitor(Context, $"Hub_{counterName}");
            _forwardEntry = new ProcessEntry(Self.Path.Name, "Forwarding to child", "All messages should be forwarded");

            Receive<Terminated>(t =>
                                {
                                    Guid id;
                                    if (!AggregateActorName.TryParseId(t.ActorRef.Path.Name, out id))
                                        return;
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
                                          if (!AggregateActorName.TryParseId(Sender.Path.Name, out id))
                                              return;
                                          //child was resumed from planned shutdown
                                          Children[id].Terminating = false;
                                          Stash.UnstashAll();
                                          Log.Debug("Child {id} resumed. Stashed messages will be sent to it", id);
                                      });
            Receive<CheckHealth>(s => Sender.Tell(new HealthStatus(s.Payload)));
            Receive<IMessageMetadataEnvelop>(messageWithMetadata =>
                                             {
                                                 var childId = GetChildActorId(messageWithMetadata);
                                                 var name = GetChildActorName(childId);
                                                 messageWithMetadata.Metadata.History.Add(_forwardEntry);
                                                 SendToChild(messageWithMetadata, childId, name);
                                             });
        }

        private void StashMessage(object message)
        {
            Log.Info("Actor hub {id} stashing message {messge}", GetType().Name, message);
            Stash.Stash();
        }

        protected void SendToChild(object message, Guid childId, string name)
        {
            ChildInfo knownChild;

            var childWasCreated = false;
            if (!Children.TryGetValue(childId, out knownChild))
            {
                childWasCreated = true;
                knownChild = CreateChild(name);
                Children[childId] = knownChild;
                Context.Watch(knownChild.Ref);
            }
            else
            {
                //terminating a child is quite long operation due to snapshots saving
                //it is cheaper to resume child than wait for it termination and create rom scratch
                if (knownChild.Terminating)
                {
                    StashMessage(message);
                    knownChild.Ref.Tell(CancelShutdownRequest.Instance);
                    Log.Debug(
                                 "Stashing message {msg} for child {id}. Waiting for child resume from termination",
                                 message,
                                 childId);

                    return;
                }
            }

            knownChild.LastTimeOfAccess = BusinessDateTime.UtcNow;
            knownChild.ExpiresAt = knownChild.LastTimeOfAccess + ChildMaxInactiveTime;
            SendMessageToChild(knownChild, message);

            Log.Debug("Message {msg} sent to {isknown} child {id}",
                         message,
                         childWasCreated ? "new" : "known",
                         childId);
        }

        //TODO: replace with more efficient implementation
        internal virtual TimeSpan ChildClearPeriod => _recycleConfiguration.ChildClearPeriod;
        internal virtual TimeSpan ChildMaxInactiveTime => _recycleConfiguration.ChildMaxInactiveTime;
        public IStash Stash { get; set; }

        protected abstract string GetChildActorName(Guid childId);
        protected abstract Guid GetChildActorId(IMessageMetadataEnvelop message);
        protected abstract Type ChildActorType { get; }

        protected virtual void SendMessageToChild(ChildInfo knownChild, object message)
        {
            knownChild.Ref.Tell(message);
        }

        private void Clear()
        {
            var now = BusinessDateTime.UtcNow;
            var childsToTerminate =
                Children.Where(c => now > c.Value.ExpiresAt && !c.Value.Terminating).Select(ch => ch.Key).ToArray();

            foreach (var childId in childsToTerminate)
                ShutdownChild(childId);

            Log.Debug("Clear childs process finished, removing {childsToTerminate} childs", childsToTerminate.Length);
        }

        private void ShutdownChild(Guid childId)
        {
            ChildInfo childInfo;
            if (!Children.TryGetValue(childId, out childInfo))
                return;

            childInfo.Ref.Tell(GracefullShutdownRequest.Instance);
            childInfo.Terminating = true;
        }

        protected override bool AroundReceive(Receive receive, object message)
        {
            _monitor.IncrementMessagesReceived();
            return base.AroundReceive(receive, message);
        }

        private ChildInfo CreateChild(string name)
        {
            var props = Context.DI().Props(ChildActorType);
            var childActorRef = Context.ActorOf(props, name);
            return new ChildInfo(childActorRef);
        }

        protected override void PreStart()
        {
            _monitor.IncrementActorStarted();
            Log.Debug("{ActorHub} is going to start", Self.Path);
            Context.System.Scheduler.ScheduleTellRepeatedly(ChildClearPeriod,
                                                            ChildClearPeriod,
                                                            Self,
                                                            new ClearChildren(),
                                                            Self);
        }

        protected override void PostStop()
        {
            _monitor.IncrementActorStopped();
            Log.Debug("{ActorHub} was stopped", Self.Path);
        }

        public class ClearChildren {}
    }
}