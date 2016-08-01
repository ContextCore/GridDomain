using System;
using System.Threading;
using Akka.Actor;
using CommonDomain.Core;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.Framework.Persistence;

namespace GridDomain.Tests.Framework
{
    public abstract class ExtendedNodeCommandTest : NodeCommandsTest
    {
        protected readonly bool InMemory;
        protected abstract IContainerConfiguration CreateConfiguration();
        protected abstract IMessageRouteMap CreateMap();

        protected ExtendedNodeCommandTest(bool inMemory) : 
            base( inMemory ? new AutoTestAkkaConfiguration(AkkaConfiguration.LogVerbosity.Trace).ToStandAloneInMemorySystemConfig():
                new AutoTestAkkaConfiguration().ToStandAloneSystemConfig()
                , "TestSystem", !inMemory)
        {
            InMemory = inMemory;
        }

        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {
            Func<ActorSystem[]> actorSystem = () => new [] { InMemory ? Sys : ActorSystemFactory.CreateActorSystem(akkaConf)};

            return new GridDomainNode(CreateConfiguration(),CreateMap(), actorSystem);
        }

        protected void SaveInJournal<TAggregate>(Guid id, params DomainEvent[] messages) where TAggregate : AggregateBase
        {
            string persistId = AggregateActorName.New<TAggregate>(id).ToString();
            var persistActor = GridNode.System.ActorOf(
                Props.Create(() => new PersistEventsSaveActor(persistId)), Guid.NewGuid().ToString());

            foreach (var o in messages)
                persistActor.Tell(o);

            Thread.Sleep(1000);
        }
    }
}