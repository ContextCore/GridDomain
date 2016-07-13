using System.Collections.Generic;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IProjectionGroupDescriptor
    {
        IReadOnlyCollection<MessageRoute> AcceptMessages { get; }
    }
}