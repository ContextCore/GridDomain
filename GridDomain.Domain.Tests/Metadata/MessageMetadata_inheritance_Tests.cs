using System;
using GridDomain.Common;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace GridDomain.Tests.Metadata
{
    [TestFixture]
    class MessageMetadata_inheritance_Tests
    {
        private MessageMetadata _parent;
        private MessageMetadata _child;

        [OneTimeSetUp]
        public void Metadata_should_form_correctly_on_create_child()
        {
            _parent = new Fixture().Create<MessageMetadata>();
            _child = _parent.CreateChild(Guid.NewGuid());
        }

        [Test]
        public void Child_correlationId_is_taken_from_parent_correlation_Id()
        {
            Assert.AreEqual(_parent.CorrelationId, _child.CorrelationId);
        }

        [Test]
        public void Child_casuationId_is_parent_Id()
        {
            Assert.AreEqual(_parent.MessageId, _child.CasuationId);
        }


    }
}