using GridDomain.CQRS.Messaging;
using MemBus;
using MemBus.Configurators;

namespace GridDomain.Node.MemTransportMessaging
{
    static public class MessageTransportSetup
    {
        public static IMessageTransport SetupInMemoryBus()
        {
            var bus = BusSetup.StartWith<Conservative>()
                              .Construct();

            return new MemTransportToMessageTransportAdapter(bus);
        }

    }
}