using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using KellermanSoftware.CompareNetObjects;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Dsl;
using Xunit;

namespace GridDomain.Tests.Common
{


    public static class SagaScenario
    {
        public static SagaScenario<TSaga, TState> New<TSaga, TState>() where TSaga : Process<TState> where TState : class {
            
        }

        public static SagaScenario<TSaga, TState> New<TSaga, TState>(ISagaDescriptor descriptor, 
                                                                     ISagaCreator<TState> factory = null) where TSaga : Process where TState : class, ISagaState
        {
            var factory1 = factory ?? SagaScenario<TSaga, TState>.CreateSagaFactory();
            var producer = new Saga—reatorsCatalog<TState>(descriptor, factory1);
            producer.RegisterAll(factory1);
            return new SagaScenario<TSaga, TState>(producer);
        }

        private static TFactory CreateSagaFactory()
        {
            var constructorInfo = typeof(TFactory).GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null)
                throw new CannotCreateCommandHandlerExeption();

            return (TFactory)constructorInfo.Invoke(null);
        }

        private static T CreateAggregate<T>(Guid id)
        {
            return (T)new AggregateFactory().Build(typeof(T), id, null);
        }
    }

    public class SagaScenario<TSaga, TState> where TSaga : Process<TState>
                                             where TState : class, ISagaState
    {
        internal SagaScenario(ISaga—reatorCatalog<TState> ÒreatorCatalog)
        {
            Saga—reatorCatalog = ÒreatorCatalog;
        }

        protected ISaga—reatorCatalog<TState> Saga—reatorCatalog { get; }
        public ISaga<TState> Saga { get; private set; }
        protected SagaStateAggregate<TState> SagaStateAggregate { get; private set; }

        public ICommand[] ExpectedCommands { get; private set; } = { };
        public ICommand[] ProducedCommands { get; private set; } = { };
        protected DomainEvent[] GivenEvents { get; private set; } = { };
        protected DomainEvent[] ReceivedEvents { get; private set; } = { };

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

        
        public SagaScenario<TSaga, TState> Given(params DomainEvent[] events)
        {
            GivenEvents = events;
            SagaStateAggregate = CreateAggregate<SagaStateAggregate<TState>>(Guid.NewGuid());
            SagaStateAggregate.ApplyEvents(events);
            SagaStateAggregate.ClearEvents();
            return this;
        }

        public SagaScenario<TSaga, TState> GivenState(Guid id, TState state)
        {
            InitialState = state;
            SagaStateAggregate = CreateAggregate<SagaStateAggregate<TState>>(id);
            SagaStateAggregate.ApplyEvents(new SagaCreated<TState>(state, id));
            SagaStateAggregate.ClearEvents();
            return this;
        }

        public SagaScenario<TSaga, TState> When(params DomainEvent[] events)
        {
            ReceivedEvents = events;
            return this;
        }

        public SagaScenario<TSaga, TState> Then(params Command[] expectedCommands)
        {
            ExpectedCommands = expectedCommands;
            return this;
        }

        public async Task<SagaScenario<TSaga, TState>> Run()
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

        public SagaScenario<TSaga, TState> CheckProducedCommands()
        {
            EventsExtensions.CompareCommands(ExpectedCommands, ProducedCommands);
            return this;
        }

        public SagaScenario<TSaga, TState> CheckProducedState(TState expectedState,
                                                                        CompareLogic customCompareLogic = null)
        {
            EventsExtensions.CompareState(expectedState, Saga.State, customCompareLogic);
            return this;
        }

        public SagaScenario<TSaga, TState> CheckProducedStateIsNotChanged()
        {
            EventsExtensions.CompareState(InitialState, Saga.State);
            return this;
        }

        public SagaScenario<TSaga, TState> CheckProducedStateName(string stateName)
        {
            Assert.Equal(stateName, Saga.State.CurrentStateName);
            return this;
        }

        public SagaScenario<TSaga, TState> CheckOnlyStateNameChanged(string stateName)
        {
            CheckProducedStateName(stateName);
            EventsExtensions.CompareStateWithoutName(InitialState, Saga.State);

            return this;
        }

        public SagaScenario<TSaga, TState> CheckProducedStateName(Func<TState> expectedStateProducer)
        {
            return CheckProducedState(expectedStateProducer());
        }

      
    }
}