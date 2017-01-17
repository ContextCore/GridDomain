using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;

namespace GridDomain.CQRS.Messaging
{
    public interface IPublisher
    {
        void Publish(object msg);
        void Publish(object msg, IMessageMetadata metadata);
    }
}