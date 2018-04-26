using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting {
    public class ConditionCommandExecutorTypedDecorator<T> : IConditionCommandExecutor<T> where T : class
    {
        private readonly IConditionCommandExecutor _conditionCommandExecutor;

        public ConditionCommandExecutorTypedDecorator(IConditionCommandExecutor builder)
        {
            _conditionCommandExecutor = builder;
        }

        Task<IWaitResult> IConditionCommandExecutor.Execute(TimeSpan? timeout, bool failOnAnyFault)
        {
            return _conditionCommandExecutor.Execute(timeout, failOnAnyFault);
        }

        public IConditionCommandExecutor And<TMsg>(Predicate<TMsg> filter = null) where TMsg : class
        {
            return _conditionCommandExecutor.And(filter);
        }

        public IConditionCommandExecutor Or<TMsg>(Predicate<TMsg> filter = null) where TMsg : class
        {
            return _conditionCommandExecutor.Or(filter);
        }

        async Task<IWaitResult<T>> IConditionCommandExecutor<T>.Execute(TimeSpan? timeout, bool failOnAnyFault)
        {
            var res = await _conditionCommandExecutor.Execute(timeout, failOnAnyFault);
            return new WaitResult<T>(res.All.OfType<IMessageMetadataEnvelop>().FirstOrDefault(r => !(r.Message is IFault)),
                                     res.All.OfType<IMessageMetadataEnvelop>().FirstOrDefault(r => r.Message is IFault));
        }
    }
}