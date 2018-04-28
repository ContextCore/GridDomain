using System;
using System.Reflection;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface IExpectationBuilder<out TExpectBuilder> where TExpectBuilder : IMessageFilter<TExpectBuilder>
    {
        TExpectBuilder Expect(Type type, Func<object, bool> filter = null);
    }
    
    public interface IConditionFactory<out T, out TBuilder>: IMessageFilter<TBuilder>
    {
        T Create();
    }
     
    public interface IConditionFactory<out T>:IConditionFactory<T,IConditionFactory<T>>
    {
    }
   
}