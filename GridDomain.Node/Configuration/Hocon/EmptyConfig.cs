using Akka.Configuration;

namespace GridDomain.Node.Configuration.Hocon
{
    internal class EmptyConfig : IHoconConfig
    {
        public Config Build()
        {
            return "";
        }
    }
}