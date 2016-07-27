using System;
using System.Threading;
using Akka.Actor;
using CommonDomain.Core;
using GridDomain.EventSourcing.VersionedTypeSerialization;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Persistence;

namespace GridDomain.Tests.Framework.Persistence
{
    public abstract class PersistentTest : ExtendedNodeCommandTest
    {
        public PersistentTest(bool inMemory) : base(inMemory)
        {
            
        }

        protected void SaveInJournal<TAggregate>(Guid id, params object[] messages) where TAggregate : AggregateBase
        {
            string persistId = AggregateActorName.New<TAggregate>(id).ToString();
            var persistActor = GridNode.System.ActorOf(
                Props.Create<PersistEventsSaveActor>(() => new PersistEventsSaveActor(persistId)),Guid.NewGuid().ToString());

            foreach(var o in messages)
                persistActor.Tell(o);

            Thread.Sleep(1000);
        }
    }
}