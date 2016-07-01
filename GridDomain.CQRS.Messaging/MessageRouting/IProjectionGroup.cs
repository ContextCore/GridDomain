using System.Collections.Generic;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IProjectionGroup
    {
        void Project(object message);

        IReadOnlyCollection<MessageRoute> AcceptMessages { get; }
    }
}