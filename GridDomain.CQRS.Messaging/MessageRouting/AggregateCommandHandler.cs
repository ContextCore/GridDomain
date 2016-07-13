using System;
using System.Linq.Expressions;
using System.Reflection;
using CommonDomain.Core;
using GridDomain.Common;
using Microsoft.Practices.Unity;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class AggregateCommandHandler<TAggregate> where TAggregate : AggregateBase
    {
        private readonly Func<ICommand, TAggregate, TAggregate> _executor;
        private readonly Func<ICommand, Guid> _idLocator;
        private readonly IUnityContainer _locator;

        private AggregateCommandHandler(string name, 
                                        Func<ICommand, Guid> idLocator,
                                        Func<ICommand, TAggregate, TAggregate> executor,
                                        IUnityContainer locator)
        {
            _locator = locator;
            _executor = executor;
            MachingProperty = name;
            _idLocator = idLocator;
        }

        public string MachingProperty { get; }


        public static AggregateCommandHandler<TAggregate> New<TCommand>(Expression<Func<TCommand, Guid>> idLocator,
            Action<TCommand, TAggregate> commandExecutor, IUnityContainer container) where TCommand : ICommand
        {
            return new AggregateCommandHandler<TAggregate>(MemberNameExtractor.GetName(idLocator),
                c => idLocator.Compile()((TCommand) c),
                (cmd, agr) =>
                {
                    commandExecutor((TCommand)cmd, agr);
                    return agr;
                },
                container);
        }

        public static AggregateCommandHandler<TAggregate> New<TCommand>(Expression<Func<TCommand, Guid>> idLocator,
            Func<TCommand, TAggregate> commandExecutor, IUnityContainer container)
        {
            return new AggregateCommandHandler<TAggregate>(MemberNameExtractor.GetName(idLocator),
                c => idLocator.Compile()((TCommand) c), (cmd, agr) => commandExecutor((TCommand) cmd), container);
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