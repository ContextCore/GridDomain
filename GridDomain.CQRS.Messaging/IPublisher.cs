using GridDomain.Common;

namespace GridDomain.CQRS.Messaging
{
    public interface IPublisher
    {
        void Publish(object msg);
        void Publish(object msg, IMessageMetadata metadata);
    }
}