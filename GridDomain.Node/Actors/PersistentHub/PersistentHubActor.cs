using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Event;
using Akka.Pattern;
using GridDomain.Common;
using GridDomain.Configuration;
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
        private readonly IRecycleConfiguration _recycleConfiguration;
        internal readonly IDictionary<Guid, ChildInfo> Children = new Dictionary<Guid, ChildInfo>();
        private readonly ILoggingAdapter Log = Context.GetSeriLogger();

        protected PersistentHubActor(IRecycleConfiguration recycleConfiguration, string counterName)
        {
            _recycleConfiguration = recycleConfiguration;
            _monitor = new ActorMonitor(Context, $"Hub_{counterName}");
            _forwardEntry = new ProcessEntry(Self.Path.Name, "Forwarding to child", "All messages should be forwarded");

            Receive<GracefullShutdownRequestDecline>(t =>
                                {
                                    if (!EntityActorName.TryParseId(Sender.Path.Name, out var id))
                                        return;
                                    if (Children.TryGetValue(id, out ChildInfo info))
                                    {
                                        info.Terminating = false;
                                    }                                       
                                });
            
            Receive<Terminated>(t =>
                                {
                                    if (!EntityActorName.TryParseId(t.ActorRef.Path.Name, out var id))
                                        return;
                                    if (Children.TryGetValue(id, out ChildInfo info) && info.PendingMessages.Any())
                                    {
                                        Log.Debug("Resending {messages_number} messages arrived for a child {child} when it was in terminating state",
                                            info.PendingMessages.Count, id);

                                        foreach (var msg in info.PendingMessages)
                                            Self.Tell(msg, msg.Sender);
                                        
                                        info.PendingMessages.Clear();
                                    }
                                    Children.Remove(id);
                                });

            Receive<ClearChildren>(m => Clear());
            Receive<NotifyOnPersistenceEvents>(m => SendToChild(m,m.Id,GetChildActorName(m.Id),Sender));
            Receive<ShutdownChild>(m => ShutdownChild(m.ChildId));
            Receive<WarmUpChild>(m =>
                                 {
                                     var created = InitChild(m.Id, GetChildActorName(m.Id), out var info);
                                     var sender = Sender;
                                     sender.Tell(new WarmUpResult(info, created));
                                 });

            Receive<CheckHealth>(s => Sender.Tell(new HealthStatus(s.Payload)));
            Receive<IMessageMetadataEnvelop>(messageWithMetadata =>
                                             {
                                                 var childId = GetChildActorId(messageWithMetadata);
                                                 var name = GetChildActorName(childId);
                                                 messageWithMetadata.Metadata.History.Add(_forwardEntry);
                                                 SendToChild(messageWithMetadata, childId, name, Sender);
                                             });
        }

        private bool InitChild(Guid childId, string name, out ChildInfo childInfo)
        {
            if (Children.TryGetValue(childId, out childInfo)) return false;

            childInfo = CreateChild(name);
            Children[childId] = childInfo;
            Context.Watch(childInfo.Ref);

            return true;
        }

        protected void SendToChild(object message, Guid childId, string name, IActorRef sender)
        {
            //TODO: refactor this suspicious logic of child terminaition cancel
            bool childWasCreated;
            if (!(childWasCreated = InitChild(childId, name, out var knownChild)))
            {
                if (knownChild.Terminating)
                {
                    knownChild.PendingMessages.Add(new ChildInfo.MessageWithSender(message,sender));
                    Log.Debug(
                        "Keeping message {@msg} for child {id}. Waiting for child to terminate. Message will be resent after.",
                        message,
                        childId);

                    return;
                }
            }

            knownChild.LastTimeOfAccess = BusinessDateTime.UtcNow;
            knownChild.ExpiresAt = knownChild.LastTimeOfAccess + ChildMaxInactiveTime;

            SendMessageToChild(knownChild, message, sender);
            LogMessageSentToChild(message, childId, childWasCreated);
        }

        private void LogMessageSentToChild(object message, Guid childId, bool childWasCreated)
        {
            if (message is IMessageMetadataEnvelop env)
            {
                Log.Debug("Message {msg} with metadata sent to {isknown} child {id}",
                    env.Message,
                    childWasCreated ? "new" : "known",
                    childId);
            }
            else
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

        protected virtual void SendMessageToChild(ChildInfo knownChild, object message, IActorRef sender)
        {
            knownChild.Ref.Tell(message, sender);
        }

        private void Clear()
        {
            Log.Debug("Starting children clear");

            var now = BusinessDateTime.UtcNow;
            var childsToTerminate =
                Children.Where(c => now > c.Value.ExpiresAt && !c.Value.Terminating)
                        .Select(ch => ch.Key)
                        .ToArray();

            foreach (var childId in childsToTerminate)
                ShutdownChild(childId);

            Log.Debug("Removing {childsToTerminate} of {total} children",
                      childsToTerminate.Length,
                      Children.Count);
        }

        private void ShutdownChild(Guid childId)
        {
            if (!Children.TryGetValue(childId, out var childInfo))
                return;

            childInfo.Terminating = true;
            childInfo.Ref.Tell(GracefullShutdownRequest.Instance);
        }

        protected override bool AroundReceive(Receive receive, object message)
        {
            _monitor.IncrementMessagesReceived();
            return base.AroundReceive(receive, message);
        }

        private ChildInfo CreateChild(string name)
        {
            var props = Context.DI()
                               .Props(ChildActorType);
            var childActorRef = Context.ActorOf(props, name);
            return new ChildInfo(childActorRef);
        }

        protected override void PreStart()
        {
            _monitor.IncrementActorStarted();
            Log.Debug("Starting");
            Context.System.Scheduler.ScheduleTellRepeatedly(ChildClearPeriod,
                ChildClearPeriod,
                Self,
                ClearChildren.Instance,
                Self);
        }

        protected override void PostStop()
        {
            _monitor.IncrementActorStopped();
            Log.Debug("Stopped");
        }

        private sealed class ClearChildren
        {
            private ClearChildren() { }
            public static ClearChildren Instance { get; } = new ClearChildren();
        }
    }

    
}