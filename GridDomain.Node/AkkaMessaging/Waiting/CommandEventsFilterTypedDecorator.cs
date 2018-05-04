using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting {
    public class CommandEventsFilterTypedDecorator<T> : ICommandEventsFilter<T> where T : class
    {
        private readonly ICommandEventsFilter _commandEventsFilter;

        public CommandEventsFilterTypedDecorator(ICommandEventsFilter builder)
        {
            _commandEventsFilter = builder;
        }

        Task<IWaitResult> ICommandEventsFilter.Execute(TimeSpan? timeout, bool failOnAnyFault)
        {
            return _commandEventsFilter.Execute(timeout, failOnAnyFault);
        }

        public ICommandEventsFilter And<TMsg>(Predicate<TMsg> filter = null) where TMsg : class
        {
            return _commandEventsFilter.And(filter);
        }

        public ICommandEventsFilter Or<TMsg>(Predicate<TMsg> filter = null) where TMsg : class
        {
            return _commandEventsFilter.Or(filter);
        }

        //public IReadOnlyCollection<Type> AcceptedMessageTypes => _commandEventsFilter.AcceptedMessageTypes;
        //public bool Check(params object[] messages) => _commandEventsFilter.Check(messages);

        async Task<IWaitResult<T>> ICommandEventsFilter<T>.Execute(TimeSpan? timeout, bool failOnAnyFault)
        {
            var res = await _commandEventsFilter.Execute(timeout, failOnAnyFault);
            return WaitResult.Parse<T>(res);
        }
    }
}