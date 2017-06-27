using GridDomain.CQRS.Messaging;
using GridDomain.Node.Configuration.Composition;

namespace GridDomain.Node {
    public class DomainContext : IDomainContext
    {
        public DomainContext(IPublisher transport)
        {
            Publisher = transport;
        }

        public IPublisher Publisher { get; }
    }
}