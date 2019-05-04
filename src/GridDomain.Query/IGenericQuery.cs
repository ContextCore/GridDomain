using System.Threading.Tasks;

namespace GridDomain.Query
{
    
    public interface IGenericQuery<TReturn>
    {
        Task<TReturn> Execute();
    }
    
    public interface IGenericQuery<TParam, TReturn>
    {
        Task<TReturn> Execute(TParam p1);
    }
    
    public interface IGenericQuery<TParam1, TParam2, TReturn>
    {
        Task<TReturn> Execute(TParam1 p1, TParam2 p2);
    }
}