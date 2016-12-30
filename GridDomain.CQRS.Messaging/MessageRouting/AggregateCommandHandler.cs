using System;
using System.Linq.Expressions;
using System.Reflection;
using CommonDomain.Core;
using GridDomain.Common;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class AggregateCommandHandler<TAggregate> where TAggregate : AggregateBase
    {
        private readonly Func<ICommand, TAggregate, TAggregate> _executor;
        private readonly Func<ICommand, Guid> _idLocator;

        protected AggregateCommandHandler(string name, 
                                        Func<ICommand, Guid> idLocator,
                                        Func<ICommand, TAggregate, TAggregate> executor)
        {
            _executor = executor;
            MachingProperty = name;
            _idLocator = idLocator;
        }

        public string MachingProperty { get; }


        public static AggregateCommandHandler<TAggregate> New<TCommand>(Expression<Func<TCommand, Guid>> idLocator,
            Action<TCommand, TAggregate> commandExecutor) where TCommand : ICommand
        {
            return new AggregateCommandHandler<TAggregate>(MemberNameExtractor.GetName(idLocator),
                c => idLocator.Compile()((TCommand) c),
                (cmd, agr) =>
                {
                    commandExecutor((TCommand)cmd, agr);
                    return agr;
                });
        }

        public static AggregateCommandHandler<TAggregate> New<TCommand>(Expression<Func<TCommand, Guid>> idLocator,
            Func<TCommand, TAggregate> commandExecutor)
        {
            return new AggregateCommandHandler<TAggregate>(MemberNameExtractor.GetName(idLocator),
                c => idLocator.Compile()((TCommand) c), (cmd, agr) => commandExecutor((TCommand) cmd));
        }

        public Guid GetId(ICommand command)
        {
            return _idLocator(command);
        }

        public TAggregate Execute(TAggregate agr, ICommand command)
        {
            return _executor(command, agr);
        }
    }
}