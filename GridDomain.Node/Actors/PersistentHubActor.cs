using System;
using System.Collections.Generic;
using System.Linq;
using Akka;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Event;
using Akka.Monitoring;
using GridDomain.Common;
using GridDomain.Logging;

namespace GridDomain.Node.Actors
{
    /// <summary>
    /// Any child should be terminated by ShutdownRequest message
    /// </summary>
    public abstract class PersistentHubActor: ReceiveActor
    {
        internal readonly IDictionary<Guid, ChildInfo> Children = new Dictionary<Guid, ChildInfo>();
        private readonly IPersistentChildsRecycleConfiguration _recycleConfiguration;
        //TODO: replace with more efficient implementation
        internal virtual TimeSpan ChildClearPeriod => _recycleConfiguration.ChildClearPeriod;
        internal virtual TimeSpan ChildMaxInactiveTime => _recycleConfiguration.ChildMaxInactiveTime;
        private readonly ActorMonitor _monitor;
        private readonly ILoggingAdapter Logger = Context.GetLogger();
        private readonly ProcessEntry _forwardEntry;

        protected abstract string GetChildActorName(object message);
        protected abstract Guid GetChildActorId(object message);
        protected abstract Type GetChildActorType(object message);

        protected PersistentHubActor(IPersistentChildsRecycleConfiguration recycleConfiguration, string counterName)
        {
            _recycleConfiguration = recycleConfiguration;
            _monitor = new ActorMonitor(Context, $"Hub_{counterName}");
            _forwardEntry = new ProcessEntry(Self.Path.Name, "Forwarding to child", "All messages should be forwarded");

            Receive<ClearChildren>(m => Clear());
            Receive<ShutdownChild>(m => ShutdownChild(m.ChildId));
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
                }

                knownChild.LastTimeOfAccess = BusinessDateTime.UtcNow;
                knownChild.ExpiresAt = knownChild.LastTimeOfAccess + ChildMaxInactiveTime;
                SendMessageToChild(knownChild, messageWitMetadata);

                Logger.Debug("Message {@msg} sent to {isknown} child {id}",
                              messageWitMetadata,
                              childWasCreated ? "known" : "new",
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
           var childsToTerminate = Children.Where(c => now > c.Value.ExpiresAt)
                                           .Select(ch => ch.Key)
                                           .ToArray();
           foreach (var childId in childsToTerminate)
           {
               //TODO: wait for child termination
               ShutdownChild(childId);
           }

            Logger.Debug("Clear childs process finished, removed {childsToTerminate} childs", childsToTerminate.Length);
        }

        private void ShutdownChild(Guid childId)
        {
            ChildInfo childInfo;
            if (!Children.TryGetValue(childId, out childInfo))
                return;
            childInfo.Ref.Tell(GracefullShutdownRequest.Instance);
            Children.Remove(childId);
        }

        public class ClearChildren
        {
        }

        protected override bool AroundReceive(Receive receive, object message)
        {
            _monitor.IncrementMessagesReceived();
            return base.AroundReceive(receive, message);
        }

        protected ChildInfo CreateChild(IMessageMetadataEnvelop messageWitMetadata, string name)
        {
            var childActorType = GetChildActorType(messageWitMetadata.Message);
            var props = Context.DI().Props(childActorType);
            var childActorRef = Context.ActorOf(props, name);
            return new ChildInfo(childActorRef);
        }

        protected override void PreStart()
        {
            _monitor.IncrementActorStarted();
            Logger.Debug("{ActorHub} is going to start", Self.Path);
            Context.System.Scheduler.ScheduleTellRepeatedly(ChildClearPeriod, ChildClearPeriod, Self, new ClearChildren(), Self);
        }

        protected override void PostStop()
        {
            _monitor.IncrementActorStopped();
            Logger.Debug("{ActorHub} was stopped", Self.Path);
        }
    }

    public class ShutdownChild
    {
        public ShutdownChild(Guid childId)
        {
            ChildId = childId;
        }

        public Guid ChildId { get; }
    }
}