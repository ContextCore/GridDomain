using System;
using System.Diagnostics;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.SampleDomain.Events;

namespace GridDomain.Tests.SampleDomain.ProjectionBuilders
{
    public class AggregateCreatedProjectionBuilder : IHandler<IMessageMetadataEnvelop<SampleAggregateCreatedEvent>>
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

        public virtual void Handle(IMessageMetadataEnvelop<SampleAggregateCreatedEvent> msg)
        {
            msg.Message.History.ProjectionGroupHashCode = ProjectionGroupHashCode;
            msg.Message.History.SequenceNumber = ++number;
            msg.Message.History.ElapsedTicksFromAppStart = watch.ElapsedTicks;
            msg.Message.History.HandlerName = this.GetType().Name;

            var metadata = MessageMetadata.CreateFrom(msg.Message.SourceId,
                                                      msg.Metadata,
                                                      new ProcessEntry(GetType().Name, MessageProcessed, Why));

            var notification = new AggregateCreatedEventNotification() {AggregateId = msg.Message.SourceId};
            _publisher.Publish(notification, metadata);
        }
    }
}