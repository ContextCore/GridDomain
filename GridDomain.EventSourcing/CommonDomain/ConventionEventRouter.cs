using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GridDomain.Common;

namespace GridDomain.EventSourcing.CommonDomain
{
    public sealed class ConventionEventRouter : TypeCatalog<Action<IAggregate,DomainEvent>,DomainEvent>
    {
        public ConventionEventRouter(Type aggregateType)
        {
            Register(aggregateType);
        }

        private Action<IAggregate, DomainEvent> CreateAction(Type aggregateType, MethodInfo applyMethod)
        {
            var aggregate = Expression.Parameter(typeof(IAggregate), "aggregate");
            var evt = Expression.Parameter(typeof(DomainEvent), "event");
            var domainEventType = applyMethod.GetParameters().First().ParameterType;
            //bake (agr,e) => ((TConcreteAggregate)agr).Apply((TConcreteEvent)e);
            return 
                Expression.Lambda<Action<IAggregate, DomainEvent>>(
                      Expression.Call(Expression.Convert(aggregate, aggregateType), 
                                                    applyMethod, 
                                                    Expression.Convert(evt, domainEventType)), aggregate,evt).Compile();

        }
        private void Register(Type aggregateType)
        {
            // Get instance methods named Apply with one parameter returning void
            var applyMethods = aggregateType
                                        .GetRuntimeMethods()
                                        .Where(m => m.Name == "Apply" && !m.IsGenericMethodDefinition && m.GetParameters().Length == 1 && m.ReturnParameter.ParameterType == typeof(void))
                                        .Select(m => new
                                                     {
                                                         Method = CreateAction(aggregateType,m),
                                                         MessageType = m.GetParameters().Single().ParameterType
                                                     });

            foreach (var apply in applyMethods)
            {
                Add(apply.MessageType,
                    (a,m) =>
                    {
                        try
                        {
                            apply.Method(a,m);
                        }
                        catch (TargetInvocationException ex)
                        {
                            throw ex.InnerException;
                        }
                    });
            }
        }

        public void Dispatch(IAggregate aggregate, DomainEvent eventMessage)
        {
            if (eventMessage == null)
                throw new ArgumentNullException(nameof(eventMessage));

            var handler = Get(eventMessage);
            if(handler != null)
                handler(aggregate, eventMessage);
            else
                aggregate.ThrowHandlerNotFound(eventMessage);
        }
    }
}