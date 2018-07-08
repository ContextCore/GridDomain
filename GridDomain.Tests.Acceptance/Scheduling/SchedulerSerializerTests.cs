using System;
using GridDomain.EventSourcing;
using GridDomain.Node.Serializers;
using GridDomain.Scheduling.Quartz;
using Xunit;

namespace GridDomain.Tests.Acceptance.Scheduling
{
    public class SchedulerSerializerTests
    {
        [Fact]
        public void Serializer_can_serialize_and_deserialize_polymorphic_types()
        {
            var options = new ExecutionOptions(DateTime.MaxValue,TimeSpan.FromMinutes(1));
            var serializer = new DomainSerializer();
            var bytes = serializer.ToBinary(options);
            var deserialized = (ExecutionOptions) serializer.FromBinary(bytes, typeof(ExecutionOptions));
            Assert.Equal(options.RunAt,deserialized.RunAt);
            Assert.Equal(options.Timeout,deserialized.Timeout);
        }
    }
}