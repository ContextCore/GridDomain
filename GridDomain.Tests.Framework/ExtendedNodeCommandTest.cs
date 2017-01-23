using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using CommonDomain.Core;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tools;
using GridDomain.Tools.Repositories;

namespace GridDomain.Tests.Framework
{
    public abstract class ExtendedNodeCommandTest : NodeCommandsTest
    {
        protected virtual bool InMemory { get; } = true;
        protected static readonly AkkaConfiguration AkkaCfg = new AutoTestAkkaConfiguration();
        protected abstract IContainerConfiguration CreateConfiguration();
        protected abstract IMessageRouteMap CreateMap();
        protected IPublisher Publisher => GridNode.Transport;
        protected ExtendedNodeCommandTest(bool inMemory, AkkaConfiguration cfg = null) : 
            base( inMemory ? (cfg ?? AkkaCfg).ToStandAloneInMemorySystemConfig() : (cfg ?? AkkaCfg).ToStandAloneSystemConfig()
                , AkkaCfg.Network.SystemName
                , !inMemory)
        {
            InMemory = inMemory;
        }

        protected ExtendedNodeCommandTest() : this(true)
        {

        }

        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf)
        {
            return  new GridDomainNode(CreateConfiguration(),CreateMap(),() => new [] {Sys}, 
                (InMemory ? (IQuartzConfig) new InMemoryQuartzConfig() : new PersistedQuartzConfig()));
        } 

        protected virtual async Task SaveInJournal<TAggregate>(Guid id, params DomainEvent[] messages) where TAggregate : AggregateBase
        {
            string persistId = AggregateActorName.New<TAggregate>(id).ToString();
            var persistActor = GridNode.System.ActorOf(
                Props.Create(() => new EventsRepositoryActor(persistId)), Guid.NewGuid().ToString());

            foreach (var o in messages)
                await persistActor.Ask<EventsRepositoryActor.Persisted>(new EventsRepositoryActor.Persist(o));
        }
    }
}