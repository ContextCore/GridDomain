using System;
using System.Linq;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Dsl;

namespace GridDomain.Tests.Framework
{
    public class SagaScenario<TSaga, TData, TFactory> where TSaga : SagaStateMachine<TData>
                                                      where TData : class, ISagaState
                                                      where TFactory : class,
                                                      ISagaFactory
                                                      <ISaga<TSaga, TData>, SagaStateAggregate<TData>>
    {
        internal SagaScenario(ISagaProducer<ISaga<TSaga, TData>> producer)
        {
            SagaProducer = producer;
        }

        protected ISagaProducer<ISaga<TSaga, TData>> SagaProducer { get; }
        public ISaga<TSaga, TData> Saga { get; private set; }
        protected SagaStateAggregate<TData> SagaStateAggregate { get; private set; }

        protected ICommand[] ExpectedCommands { get; private set; } = {};
        protected ICommand[] ProducedCommands { get; private set; } = {};
        protected DomainEvent[] GivenEvents { get; private set; } = {};
        protected DomainEvent[] ReceivedEvents { get; private set; } = {};

        public TData InitialState { get; private set; }

        public TData GenerateState(string stateName,
                                   Func<ICustomizationComposer<TData>, IPostprocessComposer<TData>> fixtureConfig = null)
        {
            var fixture = new Fixture();
            var composer = fixtureConfig?.Invoke(fixture.Build<TData>());
            var generateState = composer != null ? composer.Create() : fixture.Create<TData>();
            generateState.CurrentStateName = stateName;
            return generateState;
        }

        public static SagaScenario<TSaga, TData, TFactory> New(ISagaDescriptor descriptor, TFactory factory = null)
        {
            var producer = new SagaProducer<ISaga<TSaga, TData>>(descriptor);
            producer.RegisterAll<TFactory, TData>(factory ?? CreateSagaFactory());
            return new SagaScenario<TSaga, TData, TFactory>(producer);
        }

        public SagaScenario<TSaga, TData, TFactory> Given(params DomainEvent[] events)
        {
            GivenEvents = events;
            SagaStateAggregate = CreateAggregate<SagaStateAggregate<TData>>(Guid.NewGuid());
            SagaStateAggregate.ApplyEvents(events);
            SagaStateAggregate.ClearEvents();
            return this;
        }

        public SagaScenario<TSaga, TData, TFactory> GivenState(Guid id, TData state)
        {
            InitialState = state;
            SagaStateAggregate = CreateAggregate<SagaStateAggregate<TData>>(id);
            SagaStateAggregate.ApplyEvents(new SagaCreatedEvent<TData>(state, id));
            SagaStateAggregate.ClearEvents();
            return this;
        }

        public SagaScenario<TSaga, TData, TFactory> When(params DomainEvent[] events)
        {
            ReceivedEvents = events;
            return this;
        }

        public SagaScenario<TSaga, TData, TFactory> Then(params Command[] expectedCommands)
        {
            ExpectedCommands = expectedCommands;
            return this;
        }

        public SagaScenario<TSaga, TData, TFactory> Run()
        {
            if (SagaStateAggregate != null)
                Saga = SagaProducer.Create(SagaStateAggregate);

            foreach (var evt in ReceivedEvents.Where(e => SagaProducer.KnownDataTypes.Contains(e.GetType())))
                Saga = SagaProducer.Create(evt);

            //When

            foreach (var evt in ReceivedEvents)
                //cast to allow dynamic to locate Transit method
                (Saga as ISaga).CreateNextState((dynamic) evt);

            //Then
            ProducedCommands = Saga.CommandsToDispatch.ToArray();

            return this;
        }

        public SagaScenario<TSaga, TData, TFactory> CheckProducedCommands()
        {
            EventsExtensions.CompareCommands(ExpectedCommands, ProducedCommands);
            return this;
        }

        public SagaScenario<TSaga, TData, TFactory> CheckProducedState(TData expectedState,
                                                                       CompareLogic customCompareLogic = null)
        {
            EventsExtensions.CompareState(expectedState, Saga.State, customCompareLogic);
            return this;
        }

        public SagaScenario<TSaga, TData, TFactory> CheckProducedStateIsNotChanged()
        {
            EventsExtensions.CompareState(InitialState, Saga.State);
            return this;
        }

        public SagaScenario<TSaga, TData, TFactory> CheckProducedStateName(string stateName)
        {
            Assert.AreEqual(stateName, Saga.State.CurrentStateName);
            return this;
        }

        public SagaScenario<TSaga, TData, TFactory> CheckOnlyStateNameChanged(string stateName)
        {
            CheckProducedStateName(stateName);
            EventsExtensions.CompareStateWithoutName(InitialState, Saga.State);

            return this;
        }

        public SagaScenario<TSaga, TData, TFactory> CheckProducedStateName(Func<TData> expectedStateProducer)
        {
            return CheckProducedState(expectedStateProducer());
        }

        private static TFactory CreateSagaFactory()
        {
            var constructorInfo = typeof(TFactory).GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null)
                throw new CannotCreateCommandHandlerExeption();

            return (TFactory) constructorInfo.Invoke(null);
        }

        private static T CreateAggregate<T>(Guid id)
        {
            return (T) new AggregateFactory().Build(typeof(T), id, null);
        }
    }
}