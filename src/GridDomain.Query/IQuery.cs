using System.Collections.Generic;
using System.Threading.Tasks;

namespace GridDomain.Query
{
    public interface IQuery<TReturn> : IGenericQuery<IReadOnlyCollection<TReturn>>
    {
    }

    public interface IQuery<TParam, TReturn> : IGenericQuery<TParam, IReadOnlyCollection<TReturn>>
    {
    }

    public interface IQuery<TParam1, TParam2, TReturn> : IGenericQuery<TParam1, TParam2, IReadOnlyCollection<TReturn>>
    {
    }


}