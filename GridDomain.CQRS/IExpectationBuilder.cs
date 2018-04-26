using System;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface IExpectationBuilder<TExpectBuilder> where TExpectBuilder : IConditionBuilder<TExpectBuilder>
    {
       // IExpectationFactory<TMsg,  TExpectBuilder> Expect<TMsg>(Predicate<TMsg> filter = null) where TMsg : class;
        IConditionBuilder<TExpectBuilder> Expect(Type type, Func<object, bool> filter = null);
    }

    public interface IExpectationFactory<TResult, TBuilder> : IConditionBuilder<TBuilder>
    {
        Task<IWaitResult<TResult>> Create();
     //   TBuilder Builder { get; }
    }
}