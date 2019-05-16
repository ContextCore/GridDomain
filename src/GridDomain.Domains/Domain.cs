using System;
using System.Collections;
using System.Collections.Generic;
using GridDomain.Aggregates;

namespace GridDomain.Domains
{
    public class Domain:IDomain
    {
        private readonly IDictionary<Type, object> _customCommandHandlers;

        public Domain(ICommandHandler<ICommand> commandExecutor, 
                      IAggregatesController aggregatesController,
                      IDictionary<Type,object> customCommandHandlers)
        {
            _customCommandHandlers = customCommandHandlers;
            CommandExecutor = commandExecutor;
            AggregatesController = aggregatesController;
        }

        public ICommandHandler<ICommand> CommandExecutor { get; }
        public T CommandHandler<T>() where T : ICommandHandler<ICommand>
        {
            if (!_customCommandHandlers.ContainsKey(typeof(T)))
                throw new UnknownCommandExecutorException();

            var func = _customCommandHandlers[typeof(T)] as Func<ICommandHandler<ICommand>, T>;
            if(func == null)
                throw new UnknownCommandExecutorProviderException();
            
            return func.Invoke(CommandExecutor);
        }

        public IAggregatesController AggregatesController { get; }
    }

    public class UnknownCommandExecutorProviderException : Exception
    {
    }

    public class UnknownCommandExecutorException : Exception
    {
    }
}