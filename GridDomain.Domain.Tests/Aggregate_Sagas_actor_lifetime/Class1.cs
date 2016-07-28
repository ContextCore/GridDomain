using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using CommonDomain.Core;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tests.SynchroniousCommandExecute;
using NUnit.Framework;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{
    public static class GridNodeDebugExtensions
    {
        public static Task<T> ExecuteCorrelated<T>(this IGridDomainNode node, ICommand command) where T:DomainEvent
        {
            return node.Execute(command, ExpectedMessage.Once<T>(t => t.SourceId,command.Id));
        }
    }
    
    [TestFixture]
    class PersistentHub_childs_lifetime_test: InMemorySampleDomainTests
    {
        private IActorRef _hub;
        private CreateAggregateCommand _createAggregateCommand;

        class TestAggregateActor<T> : AggregateActor<T> where T : AggregateBase
        {
            public TestAggregateActor(IAggregateCommandsHandler<T> handler, AggregateFactory factory, TypedMessageActor<ScheduleCommand> schedulerActorRef, TypedMessageActor<Unschedule> unscheduleActorRef, IPublisher publisher) : base(handler, factory, schedulerActorRef, unscheduleActorRef, publisher)
            {
            }

            protected override bool Receive(object message)
            {
                Sender.Tell(new ChildEcho(message));
                return base.Receive(message);
            }
        }


        class TestAggregateHub : AggregateHubActor<SampleAggregate>
        {
            public TestAggregateHub(ICommandAggregateLocator<SampleAggregate> locator) : base(locator)
            {
            }

            protected override Type GetChildActorType(object message)
            {
                return typeof(TestAggregateActor<SampleAggregate>);
            }

            protected override TimeSpan ChildClearPeriod { get; } = TimeSpan.FromSeconds(1);
            protected override TimeSpan ChildMaxInactiveTime { get; } = TimeSpan.FromSeconds(2);
        }

        
        public void When_hub_creates_a_child()
        {
            var aggregateId = Guid.NewGuid();
            _createAggregateCommand = new CreateAggregateCommand(42,aggregateId, aggregateId);

           //TODO: place aggregate actor hub name in some place to not dublicate it
            var hubProps = GridNode.System.DI().Props<TestAggregateHub>();
            var aggregateActorName = AggregateActorName.New<SampleAggregate>(_createAggregateCommand.AggregateId);
            // GridNode.Execute(_createAggregateCommand);
            //var child = GridNode.System.ActorSelection("*/SampleAggregate").ResolveOne(TimeSpan.FromSeconds(1)).Result;
            _hub.Tell(_createAggregateCommand);

            ExpectMsg<ChildEcho>();

            Thread.Sleep(100);
        }

        public void And_it_is_not_active_until_lifetime_period_is_expired()
        {
            Thread.Sleep(2000);
        }
        public void And_it_actives_until_lifetime_period_is_expired()
        {
            Thread.Sleep(500);
            _hub.Tell(new ChangeAggregateCommand(50, _createAggregateCommand.AggregateId));
        }

        [Then]
        public void It_should_be_terminated()
        {
            When_hub_creates_a_child();
           // And_it_is_not_active_until_lifetime_period_is_expired();
            //var child  = GridNode.System.ActorSelection("*/SampleAggregate").ResolveOne(TimeSpan.FromSeconds(1)).Result;
            //Assert.Null(child);
        }

      // [Then]
      // public void Terminate_message_should_be_sent()
      // {
      //     When_hub_creates_a_child();
      //     And_it_is_not_active_until_lifetime_period_is_expired();
      //     var child = GridNode.System.ActorSelection("*/SampleAggregate").ResolveOne(TimeSpan.FromSeconds(1)).Result;
      //     Assert.Null(child);
      // }
      //
      // [Then]
      // public void It_should_be_restorable()
      // {
      //
      // }
      //
      // public void When_hub_creates_a_child_and_it_is_active()
      // {
      //
      // }
      //
      // [Then]
      // public void LifeTime_should_be_prolongated()
      // {
      //     
      // }
    }

    internal class ChildEcho
    {
        public object Message { get; set; }

        public ChildEcho(object message)
        {
            Message = message;
        }
    }
}
