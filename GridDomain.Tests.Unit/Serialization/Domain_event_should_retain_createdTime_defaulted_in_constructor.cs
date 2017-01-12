using System;
using GridDomain.EventSourcing;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Serialization
{
    [TestFixture]
    class Domain_event_should_retain_createdTime_defaulted_in_constructor
    {
        private SampleDomainEvent _original;
        private SampleDomainEvent _restored;

        class SampleDomainEvent : DomainEvent
        {
            public int Parameter { get; }

            public SampleDomainEvent(int parameter, Guid aggregateId) : base(aggregateId,null,aggregateId)
            {
                Parameter = parameter;
            }
        }

        [OneTimeSetUp]
        public void Test()
        {
            _original = (SampleDomainEvent)new SampleDomainEvent(1223, Guid.NewGuid()).CloneWithSaga(Guid.NewGuid());
            var ser = new DomainSerializer();
            var bytes = ser.ToBinary(_original);
            _restored = (SampleDomainEvent)ser.FromBinary(bytes, typeof(SampleDomainEvent));
        }

        [Test]
        public void SagaId_should_be_equal()
        {
            Assert.AreEqual(_original.SagaId, _restored.SagaId);
        }

        [Test]
        public void CreatedTime_should_be_equal()
        {
            Assert.AreEqual(_original.CreatedTime, _restored.CreatedTime);
        }

        [Test]
        public void SourceId_should_be_equal()
        {
            Assert.AreEqual(_original.SourceId, _restored.SourceId);
        }

        [Test]
        public void Parameter_should_be_equal()
        {
            Assert.AreEqual(_original.Parameter, _restored.Parameter);
        }
    }
}