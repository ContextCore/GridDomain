using System;
using System.Reflection;
using GridDomain.Common;
using GridDomain.ProcessManagers.DomainBind;

namespace GridDomain.ProcessManagers.Creation
{
    public class ProcessManager—reatorsCatalog<TState> : TypeCatalog<Func<object, Guid?, IProcessManager<TState>>, object>,
                                               IProcessManagerCreatorCatalog<TState> where TState : IProcessState
    {
        private readonly IProcessManagerCreator<TState> _fromStateCreator;
        private readonly IProcessManagerDescriptor _descriptor;

        public ProcessManager—reatorsCatalog(IProcessManagerDescriptor descriptor, IProcessManagerCreator<TState> stateCreator)
        {
            _descriptor = descriptor;
            _fromStateCreator = stateCreator;
        }

        private void Register<TMessage>(IProcessManagerCreator<TState, TMessage> factory)
        {
            Add<TMessage>((msg, id) => factory.CreateNew((TMessage) msg, id));
        }

        public IProcessManager<TState> CreateNew(object startMessage, Guid? processId = null)
        {
            var creator = Get(startMessage);
            if (creator == null)
                throw new CannotFindFactoryForProcessManagerCreation(typeof(TState), startMessage);

            return creator.Invoke(startMessage, processId);
        }

        public IProcessManager<TState> Create(TState state)
        {
            return _fromStateCreator.Create(state);
        }

        public void RegisterAll(object factory)
        {
            foreach (var startMessageType in _descriptor.StartMessages)
            {
                var expectedFactoryType = typeof(IProcessManagerCreator<,>).MakeGenericType(typeof(TState), startMessageType);
                if (!expectedFactoryType.IsInstanceOfType(factory))
                    throw new FactoryNotSupportStartMessageException(factory.GetType(), startMessageType);

                var registerMethod = typeof(ProcessManager—reatorsCatalog<TState>)
                    .GetMethod(nameof(Register), BindingFlags.Instance | BindingFlags.NonPublic)
                    .MakeGenericMethod(startMessageType);

                registerMethod.Invoke(this, new []{factory});
            }
        }

        public bool CanCreateFrom(object message)
        {
            return Catalog.ContainsKey(message.GetType());
        }
    }
}