using System;
using System.Reflection;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.Creation;
using GridDomain.ProcessManagers.DomainBind;
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
}