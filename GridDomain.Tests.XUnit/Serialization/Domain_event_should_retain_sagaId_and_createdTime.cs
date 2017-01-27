using System;
using GridDomain.EventSourcing;
using Xunit;

namespace GridDomain.Tests.XUnit.Serialization
{

    public class Domain_event_should_retain_sagaId_and_createdTime
    {
        private SampleDomainEvent _original;
        private SampleDomainEvent _restored;

        class SampleDomainEvent : DomainEvent
        {
            public int Parameter { get; }

            public SampleDomainEvent(int parameter, Guid aggregateId):base(aggregateId)
            {
                Parameter = parameter;
            }
        }

        [Fact]
        public void Test()
        {
            _original =(SampleDomainEvent) new SampleDomainEvent(1223,Guid.NewGuid()).CloneWithSaga(Guid.NewGuid());
            var ser = new DomainSerializer();
            var bytes = ser.ToBinary(_original);
            _restored = (SampleDomainEvent)ser.FromBinary(bytes, typeof(SampleDomainEvent));

       //SagaId_should_be_equal()
            Assert.Equal(_original.SagaId,_restored.SagaId); 
        //CreatedTime_should_be_equal()
            Assert.Equal(_original.CreatedTime, _restored.CreatedTime);
        //SourceId_should_be_equal()
            Assert.Equal(_original.SourceId, _restored.SourceId);
        //Parameter_should_be_equal()
            Assert.Equal(_original.Parameter, _restored.Parameter);
        }
    }
}