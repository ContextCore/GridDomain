using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting {
    public class CommandFilterTypedDecorator<T> : ICommandFilter<T> where T : class
    {
        private readonly ICommandFilter _commandFilter;

        public CommandFilterTypedDecorator(ICommandFilter builder)
        {
            _commandFilter = builder;
        }

        Task<IWaitResult> ICommandFilter.Execute(TimeSpan? timeout, bool failOnAnyFault)
        {
            return _commandFilter.Execute(timeout, failOnAnyFault);
        }

        public ICommandFilter And<TMsg>(Predicate<TMsg> filter = null) where TMsg : class
        {
            return _commandFilter.And(filter);
        }

        public ICommandFilter Or<TMsg>(Predicate<TMsg> filter = null) where TMsg : class
        {
            return _commandFilter.Or(filter);
        }

        public IReadOnlyCollection<Type> KnownMessageTypes => _commandFilter.KnownMessageTypes;
        public bool Check(params object[] messages) => _commandFilter.Check(messages);

        async Task<IWaitResult<T>> ICommandFilter<T>.Execute(TimeSpan? timeout, bool failOnAnyFault)
        {
            var res = await _commandFilter.Execute(timeout, failOnAnyFault);
            return WaitResult.Parse<T>(res);
        }
    }
}