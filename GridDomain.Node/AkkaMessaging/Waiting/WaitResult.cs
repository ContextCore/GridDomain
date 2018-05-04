using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting
{
    
    public static class ExpectationBuilderExtensions
    {
        public static IMessageConditionFactory<Task<IWaitResult<T>>> Expect<T>(this IMessageWaiter waiter, Predicate<T> predicate=null) where T : class
        {
            Func<object, bool> func = null;
            if (predicate != null)
                func = o =>
                       {
                           if (o is T t)
                               return predicate(t);
                           return false;
                       };
            
            return new TypedMessageConditionFactoryDecorator<T>(waiter.Expect(typeof(T),func));
        }
    }
    
    
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
    
    
    public class WaitResult : IWaitResult
    {
        public WaitResult(IReadOnlyCollection<object> allReceivedMessages)
        {
            All = allReceivedMessages;
        }

        public IReadOnlyCollection<object> All { get; }

        public static WaitResult<T> Parse<T>(IWaitResult res) where T : class
        {
            return new WaitResult<T>(res.All.OfType<IMessageMetadataEnvelop>().FirstOrDefault(r => !(r.Message is IFault)),
                              res.All.OfType<IMessageMetadataEnvelop>().FirstOrDefault(r => r.Message is IFault));
        }
    }

    public class WaitResult<T> : WaitResult, IWaitResult<T> where T : class
    {
        public WaitResult(IMessageMetadataEnvelop message, IMessageMetadataEnvelop fault = null):base(new object[]{ message})
        {
            Received = message?.Message as T;
            Fault = fault?.Message as IFault;
            ReceivedMetadata = message?.Metadata ?? fault?.Metadata;
        }

        public T Received { get; }
        public IMessageMetadata ReceivedMetadata { get; }
        public IFault Fault { get; }
    }

   
}