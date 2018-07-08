using System;
using System.Reflection;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface IExpectationBuilder<out TExpectBuilder> where TExpectBuilder : IMessageFilter<TExpectBuilder>
    {
        TExpectBuilder Expect(Type type, Func<object, bool> filter = null);
    }

    public interface IMessageConditionFactory<out T>:IMessageConditionFactory<T,IMessageConditionFactory<T>>
    {
    }   
}