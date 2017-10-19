using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Automatonymous;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.ProcessManagers
{
    public sealed class ConventionProcessMessageRouter<TState> : TypeCatalog<Func<Process<TState>, TState, object, Task<IReadOnlyCollection<ICommand>>>, object> where TState : class, IProcessState
    {
        public ConventionProcessMessageRouter(Type processType)
        {
            Register(processType);
        }

        private Func<Process<TState>, TState, object, Task<IReadOnlyCollection<ICommand>>> CreateTransition(Type messageType, PropertyInfo stateMachineProperty, MethodInfo openTransitMethod)
        {
            var process = Expression.Parameter(typeof(IProcess<TState>), "process");
            var state = Expression.Parameter(typeof(TState), "state");
            var message = Expression.Parameter(typeof(object), "message");

            //bake (process, state, message) => ((TConcreteProcess)process).TransitMessage<T>((Event<T>) process.Event, (T)message, state)
            // returning Task<IReadOnlyCollection<ICommand>>
            var closedTransitMethod = openTransitMethod.MakeGenericMethod(messageType);
            var processType = stateMachineProperty.DeclaringType;

            return
                Expression.Lambda<Func<Process<TState>, TState, object, Task<IReadOnlyCollection<ICommand>>>>(
                                                                                                      Expression.Call(Expression.Convert(process, processType),
                                                                                                                      closedTransitMethod,
                                                                                                                      Expression.Property(Expression.Convert(process, processType), stateMachineProperty),
                                                                                                                      Expression.Convert(message, messageType),
                                                                                                                      state),
                                                                                                      process,
                                                                                                      state,
                                                                                                      message)
                          .Compile();
        }

        private void Register(Type processType)
        {
            var methodInfo = processType.GetMethod("TransitMessage", BindingFlags.NonPublic | BindingFlags.Instance);

            // Get properties of Event<T> type 
            var applyMethods = processType.GetRuntimeProperties()
                                          .Where(m => m.PropertyType.IsConstructedGenericType && m.PropertyType.GetGenericTypeDefinition() == typeof(Event<>))
                                          .Select(m => new
                                                       {
                                                           Method = CreateTransition(m.PropertyType.GenericTypeArguments.First(),
                                                                                     m,
                                                                                     methodInfo),
                                                           MessageType = m.PropertyType.GenericTypeArguments.First()
                                                       });

            foreach (var apply in applyMethods)
            {
                Add(apply.MessageType,
                    (process, state, message) =>
                    {
                        try
                        {
                            return apply.Method(process, state, message);
                        }
                        catch (TargetInvocationException ex)
                        {
                            throw ex.InnerException;
                        }
                    });
            }
        }

        public Task<IReadOnlyCollection<ICommand>> Dispatch(Process<TState> process, TState state, object message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var handler = Get(message);
            if (handler != null)
                return handler(process, state, message);

            throw new UnbindedMessageReceivedException();
        }
    }
}