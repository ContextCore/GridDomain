using System;
using System.Reflection;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface IExpectationBuilder<TExpectBuilder> where TExpectBuilder : IConditionBuilder<TExpectBuilder>
    {
        TExpectBuilder Expect(Type type, Func<object, bool> filter = null);
    }

   
}