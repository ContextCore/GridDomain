using System.Diagnostics;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.XUnit.SampleDomain.Events;

namespace GridDomain.Tests.XUnit.SampleDomain.ProjectionBuilders
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

        private readonly IPublisher _publisher;
        public static int ProjectionGroupHashCode { get; set; }

        public AggregateCreatedProjectionBuilder(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public virtual Task Handle(SampleAggregateCreatedEvent msg, IMessageMetadata metadata)
        {
            msg.History.ProjectionGroupHashCode = ProjectionGroupHashCode;
            msg.History.ElapsedTicksFromAppStart = watch.ElapsedTicks;
            msg.History.HandlerName = this.GetType().Name;

            var notificationMetadata = MessageMetadata.CreateFrom(msg.SourceId,
                                                      metadata,
                                                      new ProcessEntry(GetType().Name, MessageProcessed, Why));

            var notification = new AggregateCreatedEventNotification() {AggregateId = msg.SourceId};
            _publisher.Publish(notification, notificationMetadata);

            return Task.CompletedTask;
        }

        public Task Handle(SampleAggregateCreatedEvent msg)
        {
           return Handle(msg, MessageMetadata.Empty);
        }
    }
}