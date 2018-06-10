using Akka.Configuration;

namespace GridDomain.Node.Configuration.Hocon
{
    public interface IHoconConfig
    {
        string Build();
    }
}
