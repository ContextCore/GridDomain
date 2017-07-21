using GridDomain.Common;

namespace GridDomain.Configuration.MessageRouting
{
    public interface IPublisher
    {
        void Publish(object msg);
        void Publish(object msg, IMessageMetadata metadata);
    }
}