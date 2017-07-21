using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Processes;
using GridDomain.Processes.Creation;
using GridDomain.Processes.DomainBind;
using GridDomain.Processes.State;
using KellermanSoftware.CompareNetObjects;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Dsl;
using Xunit;

namespace GridDomain.Tests.Common
{
    public static class SagaScenario
    {
        public static SagaScenario<TSaga, TState> New<TSaga, TState,TFactory>(IProcessManagerDescriptor descriptor)
            where TSaga : Process<TState>
            where TState : class, IProcessState
            where TFactory : IProcessManagerCreator<TState>
        {
            var factory = CreateSagaFactory<TFactory>();
            return New<TSaga,TState>(descriptor, factory);
        }

        public static SagaScenario<TSaga, TState> New<TSaga, TState>(IProcessManagerDescriptor descriptor,
                                                                     IProcessManagerCreator<TState> factory)
            where TSaga : Process<TState> 
            where TState : class, IProcessState
        {
            var producer = new ProcessManager—reatorsCatalog<TState>(descriptor, factory);
            producer.RegisterAll(factory);
            return new SagaScenario<TSaga, TState>(producer);
        }

        private static TFactory CreateSagaFactory<TFactory>()
        {
            var constructorInfo = typeof(TFactory).GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null)
                throw new CannotCreateCommandHandlerExeption();

            return (TFactory) constructorInfo.Invoke(null);
        }

      
    }

    public class SagaScenario<TSaga, TState> where TSaga : Process<TState>
                                             where TState : class, IProcessState
    {
        internal SagaScenario(IProcessManagerCreatorCatalog<TState> ŮreatorCatalog)
        {
            ProcessManager—reatorCatalog = ŮreatorCatalog;
            StateAggregate = Aggregate.Empty<ProcessStateAggregate<TState>>(Guid.NewGuid());
        }

        protected IProcessManagerCreatorCatalog<TState> ProcessManager—reatorCatalog { get; }
        public IProcessManager<TState> ProcessManager { get; private set; }
        protected ProcessStateAggregate<TState> StateAggregate { get; private set; }

        public ICommand[] ExpectedCommands { get; private set; } = { };
        public ICommand[] ProducedCommands { get; private set; } = { };
        protected DomainEvent[] GivenEvents { get; private set; } = { };
        protected DomainEvent[] ReceivedEvents { get; private set; } = { };

        public TState InitialState { get; private set; }

        public TState NewState(string stateName,
                                    Func<ICustomizationComposer<TState>, IPostprocessComposer<TState>> fixtureConfig = null)
        {
            var fixture = new Fixture();
            var composer = fixtureConfig?.Invoke(fixture.Build<TState>());
            var state = composer != null ? composer.Create() : fixture.Create<TState>();
            state.CurrentStateName = stateName;

            return state;
        }
        public SagaScenario<TSaga, TState> Given(string stateName,
                                                 Func<ICustomizationComposer<TState>, IPostprocessComposer<TState>> fixtureConfig = null)
        {
            return Given(NewState(stateName,fixtureConfig));
        }

        public SagaScenario<TSaga, TState> Given(params DomainEvent[] events)
        {
            GivenEvents = events;
            StateAggregate = Aggregate.Empty<ProcessStateAggregate<TState>>(Guid.NewGuid());
            StateAggregate.ApplyEvents(events);
            StateAggregate.ClearEvents();
            InitialState = StateAggregate.State;
            return this;
        }

        public SagaScenario<TSaga, TState> Given(TState state)
        {
            Condition.NotNull(()=>state);
            InitialState = state;
            StateAggregate = Aggregate.Empty<ProcessStateAggregate<TState>>(state.Id);
            StateAggregate.ApplyEvents(new SagaCreated<TState>(state, state.Id));
            return this;
        }

        public SagaScenario<TSaga, TState> When(params DomainEvent[] events)
        {
            ReceivedEvents = events.Select(e => e.CloneForProcess(StateAggregate.Id)).ToArray();
            return this;
        }

        public SagaScenario<TSaga, TState> Then(params Command[] expectedCommands)
        {
            ExpectedCommands = expectedCommands.Select(c => c.CloneForProcess(StateAggregate.Id)).ToArray();
            return this;
        }

        public async Task<SagaScenario<TSaga, TState>> Run()
        {
            if (StateAggregate?.State != null)
                ProcessManager = ProcessManager—reatorCatalog.Create(StateAggregate.State);

            foreach (var evt in ReceivedEvents.Where(e => ProcessManager—reatorCatalog.CanCreateFrom(e)))
                ProcessManager = ProcessManager—reatorCatalog.CreateNew(evt);

            //When
            var producedCommands = new List<Command>();
            foreach (var evt in ReceivedEvents)
                //cast to allow dynamic to locate Transit method
            {
                Task<ProcessResult<TState>> newStateFromEventTask = ProcessManager.Transit((dynamic) evt);
                var newState = await newStateFromEventTask;
                producedCommands.AddRange(newState.ProducedCommands);
                ProcessManager.State = newState.State;
            }

            //Then
            ProducedCommands = producedCommands.ToArray();

            return this;
        }

        public SagaScenario<TSaga, TState> CheckProducedStateIsNotChanged()
        {
            EventsExtensions.CompareState(InitialState, ProcessManager.State);
            return this;
        }
    }
}