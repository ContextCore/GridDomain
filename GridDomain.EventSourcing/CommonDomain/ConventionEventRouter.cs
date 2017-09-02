using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GridDomain.EventSourcing.CommonDomain
{
    public class ConventionEventRouter : IRouteEvents
    {
        private readonly IDictionary<Type, Action<object>> handlers = new Dictionary<Type, Action<object>>();
        private readonly bool throwOnApplyNotFound;
        private IAggregate registered;

        public ConventionEventRouter() : this(true) {}

        public ConventionEventRouter(bool throwOnApplyNotFound)
        {
            this.throwOnApplyNotFound = throwOnApplyNotFound;
        }

        public ConventionEventRouter(bool throwOnApplyNotFound, IAggregate aggregate) : this(throwOnApplyNotFound)
        {
            Register(aggregate);
        }

        public virtual void Register<T>(Action<T> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            Register(typeof(T), @event => handler((T) @event));
        }

        public virtual void Register(IAggregate aggregate)
        {
            registered = aggregate ?? throw new ArgumentNullException(nameof(aggregate));

            // Get instance methods named Apply with one parameter returning void
            var applyMethods = aggregate.GetType().GetRuntimeMethods()
                                        .Where(m => m.Name == "Apply" && m.GetParameters().Length == 1 && m.ReturnParameter.ParameterType == typeof(void))
                                        .Select(m => new
                                                     {
                                                         Method = m,
                                                         MessageType = m.GetParameters().Single().ParameterType
                                                     });

            foreach (var apply in applyMethods)
            {
                var applyMethod = apply.Method;
                handlers.Add(apply.MessageType,
                    m =>
                    {
                        try
                        {
                            applyMethod.Invoke(aggregate, new[] {m});
                        }
                        catch (TargetInvocationException ex)
                        {
                            throw ex.InnerException;
                        }
                    });
            }
        }

        public virtual void Dispatch(object eventMessage)
        {
            if (eventMessage == null)
                throw new ArgumentNullException(nameof(eventMessage));

            if(handlers.TryGetValue(eventMessage.GetType(), out Action<object> handler))
                handler(eventMessage);
            else
                if(throwOnApplyNotFound)
                registered.ThrowHandlerNotFound(eventMessage);
        }

        private void Register(Type messageType, Action<object> handler)
        {
            handlers[messageType] = handler;
        }
    }
}