using System;
using GridDomain.ProcessManagers;
using GridDomain.ProcessManagers.Creation;
using GridDomain.ProcessManagers.DomainBind;

namespace GridDomain.Tests.Common {
    public static class ProcessScenario
    {
        public static ProcessScenario<TProcess, TState> New<TProcess, TState,TFactory>(IProcessManagerDescriptor descriptor)
            where TProcess : Process<TState>
            where TState : class, IProcessState
            where TFactory : IProcessManagerCreator<TState>
        {
            var factory = CreateProcessFactory<TFactory>();
            return New<TProcess,TState>(descriptor, factory);
        }

        public static ProcessScenario<TProcess, TState> New<TProcess, TState>(IProcessManagerDescriptor descriptor,
                                                                              IProcessManagerCreator<TState> factory)
            where TProcess : Process<TState> 
            where TState : class, IProcessState
        {
            var producer = new ProcessManager—reatorsCatalog<TState>(descriptor, factory);
            producer.RegisterAll(factory);
            return new ProcessScenario<TProcess, TState>(producer);
        }

        private static TFactory CreateProcessFactory<TFactory>()
        {
            var constructorInfo = typeof(TFactory).GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null)
                throw new CannotCreateCommandHandlerExeption();

            return (TFactory) constructorInfo.Invoke(null);
        }

      
    }
}