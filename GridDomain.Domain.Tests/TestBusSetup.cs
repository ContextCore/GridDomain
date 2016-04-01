using GridDomain.Node.MemTransportMessaging;
using MemBus;
using MemBus.Configurators;

namespace GridDomain.Domain.Tests
{
    public static class TestBusSetup
    {
        public static MemTransportToMessageTransportAdapter CreateMemoryMessageTransport()
        {
            var memBus = BusSetup.StartWith<Conservative>().Construct();
            return new MemTransportToMessageTransportAdapter(memBus);
        }
    }
}