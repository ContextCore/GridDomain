using System;
using GridDomain.Common;
using Ploeh.AutoFixture;
using Xunit;

namespace GridDomain.Tests.Unit.Metadata
{
    public class MessageMetadata_inheritance_Tests
    {
        private MessageMetadata _parent;
        private MessageMetadata _child;

        [Fact]
        public void Metadata_should_form_correctly_on_create_child()
        {
            _parent = new Fixture().Create<MessageMetadata>();
            _child = _parent.CreateChild(Guid.NewGuid().ToString());
            //Child_correlationId_is_taken_from_parent_correlation_Id()
            Assert.Equal(_parent.CorrelationId, _child.CorrelationId);
            //Child_casuationId_is_parent_Id()
            Assert.Equal(_parent.MessageId, _child.CasuationId);
        }
    }
}