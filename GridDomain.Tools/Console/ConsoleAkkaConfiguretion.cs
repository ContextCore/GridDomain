using GridDomain.Node.Configuration.Akka;

namespace GridDomain.Tools.Console
{
    class ConsoleAkkaConfiguretion : AkkaConfiguration
    {
        public ConsoleAkkaConfiguretion() : 
            base(new AkkaNetworkAddress("GridNodeConnector","127.0.0.1",0),
                new ConsoleDbConfig())
        {
        }
    }
}