using System.Threading.Tasks;
using Akka.Actor;

namespace GridDomain.Node
{
    public interface IActorSystemFactory
    {
        ActorSystem CreateSystem();
    }
}