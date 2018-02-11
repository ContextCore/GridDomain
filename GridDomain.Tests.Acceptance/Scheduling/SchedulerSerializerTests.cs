using System;
using GridDomain.EventSourcing;
using GridDomain.Node.Serializers;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Quartz;
using Xunit;

namespace GridDomain.Tests.Acceptance.Scheduling
{
    public class SchedulerSerializerTests
    {
        [Fact]
        public void Serializer_can_serialize_and_deserialize_polymorphic_types()
        {
            var withType = new ExecutionOptions(DateTime.MaxValue, typeof(ScheduledCommandSuccessfullyProcessed),Guid.NewGuid().ToString(),TimeSpan.FromMinutes(1));
            var serializer = new DomainSerializer();
            var bytes = serializer.ToBinary(withType);
            var deserialized = (ExecutionOptions) serializer.FromBinary(bytes, typeof(ExecutionOptions));
            Assert.True(deserialized.SuccesEventType == withType.SuccesEventType);
        }
    }
}