using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting {
    public class CommandConditionBuilderTypedDecorator<T> : ICommandConditionBuilder<T> where T : class
    {
        private readonly ICommandConditionBuilder _commandConditionBuilder;

        public CommandConditionBuilderTypedDecorator(ICommandConditionBuilder builder)
        {
            _commandConditionBuilder = builder;
        }

        Task<IWaitResult> ICommandConditionBuilder.Execute(TimeSpan? timeout, bool failOnAnyFault)
        {
            return _commandConditionBuilder.Execute(timeout, failOnAnyFault);
        }

        public ICommandConditionBuilder And<TMsg>(Predicate<TMsg> filter = null)
        {
            return _commandConditionBuilder.And(filter);
        }

        public ICommandConditionBuilder Or<TMsg>(Predicate<TMsg> filter = null)
        {
            return _commandConditionBuilder.Or(filter);
        }

        async Task<IWaitResult<T>> ICommandConditionBuilder<T>.Execute(TimeSpan? timeout, bool failOnAnyFault)
        {
            var res = await _commandConditionBuilder.Execute(timeout, failOnAnyFault);
            return new WaitResult<T>(res.All.OfType<IMessageMetadataEnvelop<T>>().FirstOrDefault(),
                res.All.OfType<IMessageMetadataEnvelop<IFault>>().FirstOrDefault());
        }
    }
}