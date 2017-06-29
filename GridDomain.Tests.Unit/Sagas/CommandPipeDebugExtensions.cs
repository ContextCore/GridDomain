using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Node;
using GridDomain.Node.Actors.CommandPipe;

namespace GridDomain.Tests.Unit.Sagas
{
    public static class CommandPipeDebugExtensions
    {
        public static async Task SetCommandActorForSagas(this Node.CommandPipe pipe, IActorRef actor)
        {
            await pipe.SagaProcessor.Ask<Initialized>(new Initialize(actor));
        }
    }
}