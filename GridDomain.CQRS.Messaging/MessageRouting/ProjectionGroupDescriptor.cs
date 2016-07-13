using System.Collections.Generic;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class ProjectionGroupDescriptor : IProjectionGroupDescriptor
    {
        private readonly List<MessageRoute> routes = new List<MessageRoute>();
        public void Add(MessageRoute route)
        {
            routes.Add(route);
        }

        public IReadOnlyCollection<MessageRoute> AcceptMessages => routes;
    }
}