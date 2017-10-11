using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using GridDomain.Common;

namespace GridDomain.ProcessManagers {
    public sealed class ConventionProcessMessageRouter<TState> : TypeCatalog<Func<Process<TState>, TState, object, Task<ProcessResult<TState>>>, object> where TState : class, IProcessState
    {
        public ConventionProcessMessageRouter(Type processType)
        {
            Register(processType);
        }

        private Func<Process<TState>, TState, object, Task<ProcessResult<TState>>> CreateTransition(Type messageType, PropertyInfo stateMachineProperty, MethodInfo openTransitMethod)
        {
            var process = Expression.Parameter(typeof(IProcess<TState>), "process");
            var state = Expression.Parameter(typeof(TState), "state");
            var message = Expression.Parameter(typeof(object), "message");
            
            //bake (process, state, message) => process.TransitMessage<T>((Event<T>) process.Event, (T)message, state)
            // returning Task<ProcessResult<T>>
            var closedTransitMethod = openTransitMethod.MakeGenericMethod(messageType);
            return
                Expression.Lambda<Func<Process<TState>, TState, object, Task<ProcessResult<TState>>>>(
                                                                                                      Expression.Call(process,
                                                                                                                      closedTransitMethod,
                                                                                                                      Expression.Property(process, stateMachineProperty),
                                                                                                                      Expression.Convert(message, messageType),
                                                                                                                      state),
                                                                                                      process,
                                                                                                      state,
                                                                                                      message)
                          .Compile();
        }

        private void Register(Type processorType)
        {
            var methodInfo = processorType.GetMethod("TransitMessage");

            // Get properties of Event<T> type 
            var applyMethods = processorType
                .GetRuntimeProperties()
                .Where(m => m.PropertyType.IsConstructedGenericType && m.PropertyType.GetGenericTypeDefinition().GenericTypeArguments.Length == 1)
                .Select(m => new
                             {
                                 Method = CreateTransition(m.PropertyType.GetGenericTypeDefinition().GenericTypeArguments.First(), m,methodInfo),
                                 MessageType = m.PropertyType.GetGenericTypeDefinition().GenericTypeArguments.First()
                             });
        
            foreach(var apply in applyMethods)
            {
                Add(apply.MessageType,
                    (process, state, message) =>
                    {
                        try
                        {
                            return apply.Method(process, state, message);
                        }
                        catch(TargetInvocationException ex)
                        {
                            throw ex.InnerException;
                        }
                    });
            }
        }

        public Task<ProcessResult<TState>> Dispatch(Process<TState> process, TState state, object message)
        {
            if(message == null)
                throw new ArgumentNullException(nameof(message));

            var handler = Get(message);
            if(handler != null)
                return handler(process, state, message);

            throw new UnbindedMessageReceivedException();
        }
    }
}