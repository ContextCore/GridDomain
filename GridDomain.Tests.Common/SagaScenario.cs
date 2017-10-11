using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.Creation;
using GridDomain.ProcessManagers.State;
using GridDomain.Tools;
using KellermanSoftware.CompareNetObjects;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Dsl;
using Xunit;

namespace GridDomain.Tests.Common
{
    public class ProcessScenario<TState> where TState : class, IProcessState
    {
        private IProcessStateFactory<TState> StateFactory { get; }

        internal ProcessScenario(IProcess<TState> process, IProcessStateFactory<TState> factory)
        {
            StateFactory = factory;
            Process = process;
            StateAggregate = AggregateFactory.BuildEmpty<ProcessStateAggregate<TState>>(Guid.NewGuid());
        }

        protected IProcess<TState> Process { get; }
        public TState State { get; private set; }
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
        public ProcessScenario<TState> Given(string stateName,
                                                 Func<ICustomizationComposer<TState>, IPostprocessComposer<TState>> fixtureConfig = null)
        {
            return Given(NewState(stateName,fixtureConfig));
        }

        public ProcessScenario<TState> Given(params DomainEvent[] events)
        {
            GivenEvents = events;
            StateAggregate = AggregateFactory.BuildEmpty<ProcessStateAggregate<TState>>(Guid.NewGuid());
            StateAggregate.ApplyEvents(events);
            StateAggregate.CommitAll();
            InitialState = StateAggregate.State;
            return this;
        }

        public ProcessScenario<TState> Given(TState state)
        {
            Condition.NotNull(()=>state);
            InitialState = state;
            StateAggregate = AggregateFactory.BuildEmpty<ProcessStateAggregate<TState>>(state.Id);
            StateAggregate.ApplyEvents(new ProcessManagerCreated<TState>(state, state.Id));
            return this;
        }

        public ProcessScenario<TState> When(params DomainEvent[] events)
        {
            ReceivedEvents = events.Select(e => e.CloneForProcess(StateAggregate.Id)).ToArray();
            return this;
        }

        public ProcessScenario<TState> Then(params Command[] expectedCommands)
        {
            ExpectedCommands = expectedCommands.Select(c => c.CloneForProcess(StateAggregate.Id)).ToArray();
            return this;
        }

        public async Task<ProcessScenario<TState>> Run()
        {
            State = StateAggregate?.State;
            
            //When
            var producedCommands = new List<Command>();
            foreach (var evt in ReceivedEvents)
                
            {
                var newState = await Process.Transit(evt,State);
                producedCommands.AddRange(newState.ProducedCommands);
                State = newState.State;
            }

            //Then
            ProducedCommands = producedCommands.ToArray();

            return this;
        }

        public ProcessScenario<TState> CheckProducedStateIsNotChanged()
        {
            EventsExtensions.CompareState(InitialState, State);
            return this;
        }
    }
}