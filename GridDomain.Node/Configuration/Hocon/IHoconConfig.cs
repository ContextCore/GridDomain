using Akka.Configuration;

namespace GridDomain.Node.Configuration.Hocon
{
    public interface IHoconConfig
    {
        Config Build();
    }
}
