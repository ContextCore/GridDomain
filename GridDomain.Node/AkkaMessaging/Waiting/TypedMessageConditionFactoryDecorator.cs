using System;
using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting {
    public class TypedMessageConditionFactoryDecorator<T> : IMessageConditionFactory<Task<IWaitResult<T>>> where T : class
    {
        private readonly IMessageConditionFactory<Task<IWaitResult>> _messageConditionFactory;

        public TypedMessageConditionFactoryDecorator(IMessageConditionFactory<Task<IWaitResult>> factory)
        {
            _messageConditionFactory = factory;
        }

        public IMessageConditionFactory<Task<IWaitResult<T>>> And<TMsg>(Predicate<TMsg> filter = null) where TMsg : class
        {
            _messageConditionFactory.And(filter);
            return this;
        }

        public IMessageConditionFactory<Task<IWaitResult<T>>> Or<TMsg>(Predicate<TMsg> filter = null) where TMsg : class
        {
            _messageConditionFactory.Or(filter);
            return this;
        }

        
        public async Task<IWaitResult<T>> Create()
        {
            var res = await _messageConditionFactory.Create();
            return  WaitResult.Parse<T>(res); 
        }

    }
}