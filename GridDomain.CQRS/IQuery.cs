using System.Collections.Generic;
using System.Threading.Tasks;

namespace GridDomain.CQRS
{
    public interface IQuery<TReturn> : IGenericQuery<IReadOnlyCollection<TReturn>> {}

    public interface IQuery<TParam, TReturn> : IGenericQuery<TParam, IReadOnlyCollection<TReturn>> {}

    public interface IQuery<TParam1, TParam2, TReturn> : IGenericQuery<TParam1, TParam2, IReadOnlyCollection<TReturn>> {}

    public interface ISingleQuery<TParam, TReturn> : IGenericQuery<TParam, TReturn> {}

    public interface ISingleQuery<TParam1, TParam2, TReturn> : IGenericQuery<TParam1, TParam2, TReturn> {}

    public interface IGenericQuery<TParam, TReturn>
    {
        Task<TReturn> Execute(TParam p1);
    }

    public interface IGenericQuery<TParam1, TParam2, TReturn>
    {
        Task<TReturn> Execute(TParam1 p1, TParam2 p2);
    }
}