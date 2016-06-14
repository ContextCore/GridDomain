using System;
using CommonDomain.Core;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Scheduling.Akka.Messages;
using SchedulerDemo.Actors;
using SchedulerDemo.Messages;
using SchedulerDemo.ScheduledMessages;

namespace SchedulerDemo.DemoSaga
{
    public class DemoRouting : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            router.RegisterSaga(DemoSaga.SagaDescriptor);
            router.RegisterAggregate<DemoAggregate, DemoAggregateCommadHandler>();
            router.RegisterAggregate<ConsoleWriterAggregate, ConsoleWriterCommandHandlerAggregate>();

        }
    }

    public class DemoAggregateCommadHandler : AggregateCommandsHandler<DemoAggregate>
    {
        public DemoAggregateCommadHandler()
        {
            Map<DemoCommand>(c => c.Guid, (c, a) =>
            {
                a.DemoMethod();
                c.Manager.Tell(new MessageSuccessfullyProcessed(c.TaskId), c.Manager);
            });
        }
    }

    public class DemoAggregate : AggregateBase
    {
        private DemoAggregate(Guid id)
        {
            Id = id;
        }

        public DemoAggregate()
        {
            Id = Guid.Empty;
        }

        public void DemoMethod()
        {
            RaiseEvent(new DemoEvent(Id));
        }

        private void Apply(DemoEvent e)
        {
            int b = 1;
        }

    }

    public class DemoSagaStateAggregate : SagaStateAggregate<DemoSaga.States, DemoSaga.Transitions>
    {

        public DemoSagaStateAggregate(Guid id) : base(id)
        {
        }

        public DemoSagaStateAggregate(Guid id, DemoSaga.States state) : base(id, state)
        {
        }
    }

    public class DemoSaga : StateSaga<DemoSaga.States, DemoSaga.Transitions, DemoSagaStateAggregate, DemoEvent>
    {
        public enum States
        {
            First,
            Second
        }

        public enum Transitions
        {
            MoveToSecond
        }

        public DemoSaga(DemoSagaStateAggregate state) : base(state)
        {
            State.MachineState = States.First;
            RegisterEvent<DemoEvent>(Transitions.MoveToSecond);

            Machine.Configure(States.First)
                   .Permit(Transitions.MoveToSecond, States.Second);

            Machine.Configure(States.Second)
                   .OnEntry(e => Dispatch(new WriteToConsole("123", "123")));
        }
        public static ISagaDescriptor SagaDescriptor => new DemoSaga(new DemoSagaStateAggregate(Guid.Empty, States.First));

        public void Handle(DemoEvent msg)
        {
            Transit(msg);
        }
    }
}
