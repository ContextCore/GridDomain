using System;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface ICommandExpectationBuilder: IExpectationBuilder<IConditionCommandExecutor>
    {
        IConditionCommandExecutor<TMsg> Expect<TMsg>(Predicate<TMsg> filter = null) where TMsg : class; 
        //IConditionCommandExecutor Expect(Type type, Func<object, bool> filter = null);
    }
    
    //public interface IExpectationBuilder<TResult, out TExpectBuilder> where TExpectBuilder : IConditionBuilder<TExpectBuilder>
    //{
    //    TExpectBuilder Expect<TMsg>(Predicate<TMsg> filter = null) where TMsg : class;
    //    //   TExpectBuilder Expect(Type type, Func<object, bool> filter = null);
    //}
}