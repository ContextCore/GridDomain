using System;
using Automatonymous;
using Automatonymous.Activities;
using Automatonymous.Binders;

namespace GridDomain.EventSourcing.Sagas.InstanceSagas
{
    public static class AutomatonymousExtensions
    {
        /// <summary>
        /// Adds a synchronous delegate activity to the event's behavior with mapping to domain event and state
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <typeparam name="TData">The event data type</typeparam>
        /// <param name="binder">The event binder</param>
        /// <param name="action">The synchronous delegate</param>
        public static EventActivityBinder<TInstance, TData> Then<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> binder, Action<TInstance, TData> action) where TInstance : class
        {
            return binder.Add((Activity<TInstance, TData>)new ActionActivity<TInstance, TData>(ctx => action(ctx.Instance, ctx.Data)));
        }
    }
}