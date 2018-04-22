using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Persistence;
using Akka.TestKit.Xunit2;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.EventSourced.Messages;

namespace GridDomain.Tests.Unit
{
    public static class TestKitExtensions
    {
        public static async Task<T> LoadActor<T>(this TestKit kit, string name) where T : ActorBase
        {
            var props = kit.Sys.DI()
                           .Props<T>();

            var actor = kit.ActorOfAsTestActorRef<T>(props, name);

            await actor.Ask<RecoveryCompleted>(NotifyOnPersistenceEvents.Instance, TimeSpan.FromSeconds(5));

            return actor.UnderlyingActor;
        }
    }
}