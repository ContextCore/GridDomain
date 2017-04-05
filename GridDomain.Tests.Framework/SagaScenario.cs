using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class SagaScenario<TSaga, TState, TFactory> where TSaga : Process<TState>
                                                      where TState : class, ISagaState
                                                      where TFactory : class, ISagaCreator<TState>
    {
        internal SagaScenario(ISaga—reatorCatalog<TState> ÒreatorCatalog)
        {
            Saga—reatorCatalog = ÒreatorCatalog;
        }

        protected ISaga—reatorCatalog<TState> Saga—reatorCatalog { get; }
        public ISaga<TState> Saga { get; private set; }
        protected SagaStateAggregate<TState> SagaStateAggregate { get; private set; }

        public ICommand[] ExpectedCommands { get; private set; } = {};
        public ICommand[] ProducedCommands { get; private set; } = {};
        protected DomainEvent[] GivenEvents { get; private set; } = {};
        protected DomainEvent[] ReceivedEvents { get; private set; } = {};

        public TState InitialState { get; private set; }

        public TState GenerateState(string stateName,
                                   Func<ICustomizationComposer<TState>, IPostprocessComposer<TState>> fixtureConfig = null)
        {
            var fixture = new Fixture();
            var composer = fixtureConfig?.Invoke(fixture.Build<TState>());
            var generateState = composer != null ? composer.Create() : fixture.Create<TState>();
            generateState.CurrentStateName = stateName;
            return generateState;
        }

        public static SagaScenario<TSaga, TState, TFactory> New(ISagaDescriptor descriptor, TFactory factory = null)
        {
            var factory1 = factory ?? CreateSagaFactory();
            var producer = new Saga—reatorsCatalog<TState>(descriptor, factory1);
            producer.RegisterAll(factory1);
            return new SagaScenario<TSaga, TState, TFactory>(producer);
        }

        public SagaScenario<TSaga, TState, TFactory> Given(params DomainEvent[] events)
        {
            GivenEvents = events;
            SagaStateAggregate = CreateAggregate<SagaStateAggregate<TState>>(Guid.NewGuid());
            SagaStateAggregate.ApplyEvents(events);
            SagaStateAggregate.ClearEvents();
            return this;
        }

        public SagaScenario<TSaga, TState, TFactory> GivenState(Guid id, TState state)
        {
            InitialState = state;
            SagaStateAggregate = CreateAggregate<SagaStateAggregate<TState>>(id);
            SagaStateAggregate.ApplyEvents(new SagaCreated<TState>(state, id));
            SagaStateAggregate.ClearEvents();
            return this;
        }

        public SagaScenario<TSaga, TState, TFactory> When(params DomainEvent[] events)
        {
            ReceivedEvents = events;
            return this;
        }

        public SagaScenario<TSaga, TState, TFactory> Then(params Command[] expectedCommands)
        {
            ExpectedCommands = expectedCommands;
            return this;
        }

        public async Task<SagaScenario<TSaga, TState, TFactory>> Run()
        {
            if (SagaStateAggregate != null)
                Saga = Saga—reatorCatalog.CreateNew(SagaStateAggregate);

            foreach (var evt in ReceivedEvents.Where(e => Saga—reatorCatalog.CanCreateFrom(e)))
                Saga = Saga—reatorCatalog.CreateNew(evt);

            //When
            var producedCommands = new List<Command>();
            foreach (var evt in ReceivedEvents)
                //cast to allow dynamic to locate Transit method
            {
                Task<TransitionResult<TState>> newStateFromEventTask = Saga.PreviewTransit((dynamic) evt);
                var newState = await newStateFromEventTask;
                producedCommands.AddRange(newState.ProducedCommands);
                Saga.ApplyTransit(newState.State);
            }

            //Then
            ProducedCommands = producedCommands.ToArray();

            return this;
        }

        public SagaScenario<TSaga, TState, TFactory> CheckProducedCommands()
        {
            EventsExtensions.CompareCommands(ExpectedCommands, ProducedCommands);
            return this;
        }

        public SagaScenario<TSaga, TState, TFactory> CheckProducedState(TState expectedState,
                                                                       CompareLogic customCompareLogic = null)
        {
            EventsExtensions.CompareState(expectedState, Saga.State, customCompareLogic);
            return this;
        }

        public SagaScenario<TSaga, TState, TFactory> CheckProducedStateIsNotChanged()
        {
            EventsExtensions.CompareState(InitialState, Saga.State);
            return this;
        }

        public SagaScenario<TSaga, TState, TFactory> CheckProducedStateName(string stateName)
        {
            Assert.AreEqual(stateName, Saga.State.CurrentStateName);
            return this;
        }

        public SagaScenario<TSaga, TState, TFactory> CheckOnlyStateNameChanged(string stateName)
        {
            CheckProducedStateName(stateName);
            EventsExtensions.CompareStateWithoutName(InitialState, Saga.State);

            return this;
        }

        public SagaScenario<TSaga, TState, TFactory> CheckProducedStateName(Func<TState> expectedStateProducer)
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