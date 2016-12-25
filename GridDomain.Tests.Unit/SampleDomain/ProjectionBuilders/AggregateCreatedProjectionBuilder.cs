using System.Diagnostics;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.Unit.SampleDomain.Events;

namespace GridDomain.Tests.Unit.SampleDomain.ProjectionBuilders
{
    public class AggregateCreatedProjectionBuilder : IHandlerWithMetadata<SampleAggregateCreatedEvent>
    {
        public const string MessageProcessed = "message processed";
        public const string Why = "message received";

        private static Stopwatch watch = new Stopwatch();
        static AggregateCreatedProjectionBuilder()
        {
            watch.Start();
        }

        private int number = 0;
        private readonly IPublisher _publisher;
        public static int ProjectionGroupHashCode { get; set; }

        public AggregateCreatedProjectionBuilder(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public virtual void Handle(SampleAggregateCreatedEvent msg, IMessageMetadata metadata)
        {
            msg.History.ProjectionGroupHashCode = ProjectionGroupHashCode;
            msg.History.ElapsedTicksFromAppStart = watch.ElapsedTicks;
            msg.History.HandlerName = this.GetType().Name;

            var notificationMetadata = MessageMetadata.CreateFrom(msg.SourceId,
                                                      metadata,
                                                      new ProcessEntry(GetType().Name, MessageProcessed, Why));

            var notification = new AggregateCreatedEventNotification() {AggregateId = msg.SourceId};
            _publisher.Publish(notification, notificationMetadata);
        }

        public void Handle(SampleAggregateCreatedEvent msg)
        {
            Handle(msg, MessageMetadata.Empty());
        }
    }
}