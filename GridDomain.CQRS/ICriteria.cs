using System.Linq;

namespace GridDomain.CQRS
{
    public interface ICriteria<T>
    {
        IQueryable<T> Apply();
    }

    public interface ICriteria<T,U>
    {
        IQueryable<T> Apply(U namePart);
    }

    public interface ICriteria<T,U,V>
    {
        IQueryable<T> Execute(U p1, V p2);
    }

    public interface ICriteria<T,U,V,K>
    {
        IQueryable<T> Execute(U p1, V p2, K p3);
    }
}