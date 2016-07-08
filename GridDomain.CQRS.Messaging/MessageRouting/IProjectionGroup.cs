using System.Collections.Generic;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IProjectionGroupDescriptor
    {
        IReadOnlyCollection<MessageRoute> AcceptMessages { get; }
    }

    public interface IProjectionGroup : IProjectionGroupDescriptor
    {
        void Project(object message);
    }

}