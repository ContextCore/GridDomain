using System;
using System.Collections;
using System.Collections.Generic;
using GridDomain.Aggregates;
using GridDomain.Aggregates.Abstractions;

namespace GridDomain.Domains
{
    public class AggregatesGateway:IAggregatesGateway
    {
        private readonly IDictionary<Type, object> _customCommandHandlers;

        public AggregatesGateway(ICommandHandler<ICommand> commandExecutor,
                      IDictionary<Type,object> customCommandHandlers=null)
        {
            _customCommandHandlers = customCommandHandlers ?? new Dictionary<Type, object>();
            CommandExecutor = commandExecutor;
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
    }

    public class UnknownCommandExecutorProviderException : Exception
    {
    }

    public class UnknownCommandExecutorException : Exception
    {
    }
}