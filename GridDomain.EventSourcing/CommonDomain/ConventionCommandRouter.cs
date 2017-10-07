using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing.CommonDomain {
    public class ConventionCommandRouter : TypeCatalog<Func<IAggregate, ICommand, Task>, ICommand>, IRouteCommands
    {
        public ConventionCommandRouter(IAggregate aggregate)
        {
            Register(aggregate.GetType());
        }

        private Func<IAggregate, ICommand, Task> CreateCommandHandler(Type aggregateType, MethodInfo commandExecuteMethod)
        {
            var aggregate = Expression.Parameter(typeof(IAggregate), "aggregate");
            var command = Expression.Parameter(typeof(ICommand), "command");
            var commandType = commandExecuteMethod.GetParameters().First().ParameterType;
            //bake (agr,cmd) => ((TConcreteAggregate)agr).Execute((TConcreteCommand)cmd);
            return
                Expression.Lambda<Func<IAggregate, ICommand, Task>>(
                    Expression.Call(Expression.Convert(aggregate, aggregateType),
                        commandExecuteMethod,
                        Expression.Convert(command, commandType)), aggregate, command).Compile();

        }
        private void Register(Type aggregateType)
        {
            // Get instance methods named Apply with one parameter returning void
            var executeMethods = aggregateType
                .GetRuntimeMethods()
                .Where(m => m.Name == "Execute" && m.GetParameters().Length == 1 && m.ReturnParameter.ParameterType == typeof(Task))
                .Select(m => new
                             {
                                 Method = CreateCommandHandler(aggregateType, m),
                                 MessageType = m.GetParameters().Single().ParameterType
                             });

            foreach(var executeCommand in executeMethods)
            {
                Add(executeCommand.MessageType,
                    (a, m) =>
                    {
                        try
                        {
                            return executeCommand.Method(a, m);
                        }
                        catch(TargetInvocationException ex)
                        {
                            throw ex.InnerException;
                        }
                    });
            }
        }

        public virtual Task Dispatch(IAggregate aggregate, ICommand command)
        {
            if(command == null)
                throw new ArgumentNullException(nameof(command));

            return Get(command)(aggregate, command);
        }
    }
}