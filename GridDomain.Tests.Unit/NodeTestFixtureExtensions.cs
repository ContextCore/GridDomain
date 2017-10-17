using System;
using Akka.TestKit.TestActors;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node.Actors.CommandPipe.Messages;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.Node;
using GridDomain.ProcessManagers.State;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Akka;
using GridDomain.Scheduling.Quartz.Configuration;
using GridDomain.Scheduling.Quartz.Retry;

namespace GridDomain.Tests.Unit
{
    public static class NodeTestFixtureExtensions
    {

        public static NodeTestFixture EnableScheduling(this NodeTestFixture fixture, IRetrySettings config )
        {
            return EnableScheduling(fixture,new InMemoryQuartzConfig(config));
        }

        public static NodeTestFixture EnableScheduling(this NodeTestFixture fixture, IQuartzConfig config = null, bool clearScheduledData = true)
        {
            IQuartzConfig quartzConfig = config ?? new InMemoryQuartzConfig(new InMemoryRetrySettings(5,TimeSpan.FromMinutes(10),new DefaultExceptionPolicy()));

            fixture.OnNodeCreatedEvent += (o, node) =>
                                                {

                                                   var ext = node.System.InitSchedulingExtension(quartzConfig,
                                                                                                 fixture.Logger,
                                                                                                 node.Transport,
                                                                                                 node);

                                                   node.DomainConfigurations.Add(new FutureAggregateHandlersDomainConfiguration(ext.SchedulingActor));
                                                };
            if(clearScheduledData)
                 fixture.OnNodeStartedEvent += (sender, args) => fixture.Node.System.GetExtension<SchedulingExtension>().Scheduler.Clear();

            return fixture;

        }

        public static NodeTestFixture UseAdaper<TFrom,TTo>(this NodeTestFixture fixture, ObjectAdapter<TFrom,TTo> adapter)
        {
            fixture.OnNodeCreatedEvent += (sender, node) => node.EventsAdaptersCatalog.Register(adapter);
            return fixture;
        }

        public static NodeTestFixture UseAdaper<TFrom,TTo>(this NodeTestFixture fixture, DomainEventAdapter<TFrom,TTo> adapter) where TFrom : DomainEvent
                                                                                                                                where TTo : DomainEvent
        {
            fixture.OnNodeCreatedEvent += (sender, node) => node.EventsAdaptersCatalog.Register(adapter);
            return fixture;
        }

        public static NodeTestFixture IgnorePipeCommands(this NodeTestFixture fixture)
        {
            fixture.OnNodeStartedEvent += (sender, e) =>
                                          {
                                              //supress errors raised by commands not reaching aggregates
                                              var nullActor = fixture.Node.System.ActorOf(BlackHoleActor.Props);
                                              //var filterActor = fixture.Node.System.ActorOf(Props.Create(() => new FilterActor(fixture.Node.Pipe.CommandExecutor)));
                                              fixture.Node.Pipe.ProcessesPipeActor.Ask<Initialized>(new Initialize(nullActor)).Wait();
                                          };

            return fixture;
        }

        class FilterActor:ReceiveActor
        {
            public FilterActor(IActorRef recepient)
            {
                Receive<IMessageMetadataEnvelop>(m =>
                                                 {
                                                     // switch (m.Message)
                                                     // {
                                                     //         case CreateNewStateCommand<>
                                                     // }
                                                     recepient.Forward(m);
                                                 });
            }    
        }
    }
}