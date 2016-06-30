using System.Collections.Generic;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IProjectionGroup
    {
        void Project(object message);

        IReadOnlyCollection<MessageRoute> AcceptMessages { get; }
    }
}