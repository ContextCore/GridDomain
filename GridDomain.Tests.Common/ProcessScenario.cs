using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.Creation;
using GridDomain.ProcessManagers.DomainBind;
using GridDomain.ProcessManagers.State;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Dsl;
using Remotion.Linq.Parsing.Structure.ExpressionTreeProcessors;

namespace GridDomain.Tests.Common {
    public static class ProcessScenario
    {
        public static ProcessScenario<TState> New<TState>(IProcess<TState> process,
                                                          IProcessStateFactory<TState> factory)
            where TState : class, IProcessState
        {
            return new ProcessScenario<TState>(process, factory);
        }

        public static ProcessScenario<TState> New<TState, TFactory>(IProcess<TState> process) where TFactory : IProcessStateFactory<TState> , new()
            where TState : class, IProcessState
        {
            return new ProcessScenario<TState>(process, new TFactory());
        }

        public static ProcessScenario<TState> New<TProcess, TState, TFactory>() where TFactory : IProcessStateFactory<TState>, new()
                                                                                where TState : class, IProcessState
                                                                                where TProcess: IProcess<TState>, new()
        {
            return New<TState,TFactory>(new TProcess());
        }
    }

    public class ProcessScenario<TState> where TState : class, IProcessState
    {
        private IProcessStateFactory<TState> StateFactory { get; }

        internal ProcessScenario(IProcess<TState> process, IProcessStateFactory<TState> factory)
        {
            StateFactory = factory;
            Process = process;
        }

        protected IProcess<TState> Process { get; }
        public TState State { get; private set; }

        public ICommand[] ExpectedCommands { get; private set; } = { };
        public ICommand[] ProducedCommands { get; private set; } = { };
        public DomainEvent[] GivenEvents { get; private set; } = { };
        public DomainEvent[] ReceivedEvents { get; private set; } = { };

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
            return Given(NewState(stateName, fixtureConfig));
        }

        public ProcessScenario<TState> Given(TState state)
        {
            Condition.NotNull(() => state);
            InitialState = state;
            return this;
        }

        public ProcessScenario<TState> When(params DomainEvent[] events)
        {
            ReceivedEvents = events.Select(e => e.CloneForProcess(Any.GUID)).ToArray();
            return this;
        }

        public ProcessScenario<TState> Then(params Command[] expectedCommands)
        {
            ExpectedCommands = expectedCommands;
            foreach (var expectedCommand in expectedCommands)
            {
                expectedCommand.ProcessId = Any.GUID;
            }
            return this;
        }

        public async Task<ProcessScenario<TState>> Run()
        {
            TState state = State;
            //When
            var producedCommands = new List<Command>();
            foreach(var evt in ReceivedEvents)

            {
                state = StateFactory.Create(evt, state);
                if(state == null) throw new ProcessStateNullException();
                var transitionResult = await Process.Transit(state, evt);

                producedCommands.AddRange(transitionResult.ProducedCommands);
            }
            State = state;
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