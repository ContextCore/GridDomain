using System;
using GridDomain.EventSourcing;
using GridDomain.Node.Serializers;
using Xunit;

namespace GridDomain.Tests.Unit.Serialization
{
    public class Domain_event_should_retain_createdTime_defaulted_in_constructor
    {
        private SampleDomainEvent _original;
        private SampleDomainEvent _restored;

        private class SampleDomainEvent : DomainEvent
        {
            public SampleDomainEvent(int parameter, string aggregateId) : base(aggregateId, processId: aggregateId, createdTime: null)
            {
                Parameter = parameter;
            }

            public int Parameter { get; }
        }

        [Fact]
        public void Test()
        {
            _original = (SampleDomainEvent) new SampleDomainEvent(1223, Guid.NewGuid().ToString()).CloneForProcess(Guid.NewGuid().ToString());
            var ser = new DomainSerializer();
            var bytes = ser.ToBinary(_original);
            _restored = (SampleDomainEvent) ser.FromBinary(bytes, typeof(SampleDomainEvent));
            //processId_should_be_equal()
            Assert.Equal(_original.ProcessId, _restored.ProcessId);
            //CreatedTime_should_be_equal()
            Assert.Equal(_original.CreatedTime, _restored.CreatedTime);
            //SourceId_should_be_equal()
            Assert.Equal(_original.SourceId, _restored.SourceId);
            //Parameter_should_be_equal()
            Assert.Equal(_original.Parameter, _restored.Parameter);
        }
    }
}