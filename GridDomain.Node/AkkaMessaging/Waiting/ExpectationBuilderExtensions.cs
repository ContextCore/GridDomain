using System;
using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging.Waiting {
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
}