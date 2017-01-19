using System;
using System.Collections.Generic;
using System.Linq;
using Akka;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Monitoring;
using GridDomain.Common;
using GridDomain.Logging;

namespace GridDomain.Node.Actors
{
    //TODO: think about replace with ConsistentHashingPool - need to deal with persistence 
    /// <summary>
    /// Any child should be terminated by ShutdownRequest message
    /// </summary>
    public abstract class PersistentHubActor: UntypedActor
    {
        internal readonly IDictionary<Guid, ChildInfo> Children = new Dictionary<Guid, ChildInfo>();
        private readonly IPersistentChildsRecycleConfiguration _recycleConfiguration;
        //TODO: replace with more efficient implementation
        internal virtual TimeSpan ChildClearPeriod => _recycleConfiguration.ChildClearPeriod;
        internal virtual TimeSpan ChildMaxInactiveTime => _recycleConfiguration.ChildMaxInactiveTime;
        protected readonly ILogger Logger = LogManager.GetLogger();
        private readonly ActorMonitor _monitor;

        protected abstract string GetChildActorName(object message);
        protected abstract Guid GetChildActorId(object message);
        protected abstract Type GetChildActorType(object message);

        public PersistentHubActor(IPersistentChildsRecycleConfiguration recycleConfiguration, string counterName)
        {
            _recycleConfiguration = recycleConfiguration;
            _monitor = new ActorMonitor(Context, $"Hub_{counterName}");
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
               Children[childId].Ref.Tell(GracefullShutdownRequest.Instance);
               Children.Remove(childId);
           }

           Logger.Trace("Clear childs process finished, removed {childsToTerminate} childs", childsToTerminate.Length);
        }

        public class ClearChilds
        {
        }

        protected override bool AroundReceive(Receive receive, object message)
        {
            return base.AroundReceive(receive, message);
        }

        protected override void Unhandled(object message)
        {
            base.Unhandled(message);
        }

        protected override void OnReceive(object msg)
        {
            _monitor.IncrementMessagesReceived();
            Logger.Trace("{ActorHub} received {@message}", Self.Path, msg);

            msg.Match()
               .With<ClearChilds>(m => Clear())
               .With<CheckHealth>(s => Sender.Tell(new HealthStatus(s.Payload)))
               .With<IMessageMetadataEnvelop>(messageWitMetadata =>
                { 
                    ChildInfo knownChild;

                    messageWitMetadata.Metadata.History.Add(new ProcessEntry(Self.Path.Name,"Forwarding to child","All messages should be forwarded"));

                    var childId = GetChildActorId(messageWitMetadata.Message);
                    var name = GetChildActorName(messageWitMetadata.Message);

                    bool childWasCreated = false;
                    if (!Children.TryGetValue(childId, out knownChild))
                    {
                        childWasCreated = true;
                        var childActorType = GetChildActorType(messageWitMetadata.Message);
                        var props = Context.DI().Props(childActorType);
                        var childActorRef = Context.ActorOf(props, name);
                        knownChild = new ChildInfo(childActorRef);
                        Children[childId] = knownChild;
                    }

                    knownChild.LastTimeOfAccess = BusinessDateTime.UtcNow;
                    knownChild.ExpiresAt = knownChild.LastTimeOfAccess + ChildMaxInactiveTime;
                    knownChild.Ref.Tell(messageWitMetadata);

                    Logger.Trace("Message {@msg} sent to {isknown} child {id}",
                                  msg,
                                  childWasCreated ? "known" : "unknown",
                                  childId);
                });
        }
        protected override void PreStart()
        {
            _monitor.IncrementActorStarted();
            Logger.Trace("{ActorHub} is going to start", Self.Path);
            Context.System.Scheduler.ScheduleTellRepeatedly(ChildClearPeriod, ChildClearPeriod, Self, new ClearChilds(), Self);
        }

        protected override void PostStop()
        {
            _monitor.IncrementActorStopped();
            Logger.Trace("{ActorHub} was stopped", Self.Path);
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                10,
                TimeSpan.FromSeconds(60),
                x =>
                {
                    if (x is NotSupportedException)
                        return Directive.Stop;
                    return Directive.Restart;
                });
        }
    }
}