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

    public static class SagaScenarioExtensions
    {
        public static async Task<SagaScenario<TSaga, TData, TFactory>> CheckProducedCommands<TSaga, TData, TFactory>(this Task<SagaScenario<TSaga, TData, TFactory>> scenarioInProgress) where TSaga : Process<TData> where TData : class, ISagaState where TFactory : class, IFactory<ISaga<TData>,TData>
        {
            var scnearion = await scenarioInProgress;
            scnearion.CheckProducedCommands();
            return scnearion;
        }

        public static async Task<SagaScenario<TSaga, TData, TFactory>> CheckOnlyStateNameChanged<TSaga, TData, TFactory>(this Task<SagaScenario<TSaga, TData, TFactory>> scenarioInProgress, string stateName) where TSaga : Process<TData> where TData : class, ISagaState where TFactory : class, IFactory<ISaga<TData>, TData>
        {
            var scnearion = await scenarioInProgress;
            scnearion.CheckOnlyStateNameChanged(stateName);
            return scnearion;
        }

        public static async Task<SagaScenario<TSaga, TData, TFactory>> CheckProducedState<TSaga, TData, TFactory>(
            this Task<SagaScenario<TSaga, TData, TFactory>> scenarioInProgress, TData expectedState, CompareLogic logic = null) where TSaga : Process<TData> where TData : class, ISagaState where TFactory : class, IFactory<ISaga<TData>, TData>
        {
            var scnearion = await scenarioInProgress;
            scnearion.CheckProducedState(expectedState, logic);
            return scnearion;
        }
    }

    public class SagaScenario<TSaga, TState, TFactory> where TSaga : Process<TState>
                                                      where TState : class, ISagaState
                                                      where TFactory : class, IFactory<ISaga<TState>, TState>
    {
        internal SagaScenario(ISagaProducer<TState> producer)
        {
            SagaProducer = producer;
        }

        protected ISagaProducer<TState> SagaProducer { get; }
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
            var producer = new SagaProducer<TState>(descriptor);
            producer.RegisterAll(factory ?? CreateSagaFactory());
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
            SagaStateAggregate.ApplyEvents(new SagaCreatedEvent<TState>(state, id));
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
                Saga = SagaProducer.Create(SagaStateAggregate);

            foreach (var evt in ReceivedEvents.Where(e => SagaProducer.KnownDataTypes.Contains(e.GetType())))
                Saga = SagaProducer.Create(evt);

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