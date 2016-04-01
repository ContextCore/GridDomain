using System.Collections.Generic;

namespace GridDomain.CQRS.Quering
{
    public interface IQuery<T> : IGenericQuery<IReadOnlyCollection<T>>
    {
    }

    public interface IQuery<T, U> : IGenericQuery<IReadOnlyCollection<T>, U>
    {
    }

    public interface IQuery<T, U, V> : IGenericQuery<IReadOnlyCollection<T>, U, V>
    {
    }


    public interface ISingleQuery<T, U> : IGenericQuery<T, U>
    {
    }

    public interface ISingleQuery<T, U, V> : IGenericQuery<T, U, V>
    {
    }


    public interface IGenericQuery<T, U>
    {
        T Execute(U p1);
    }

    public interface IGenericQuery<T, U, V>
    {
        T Execute(U p1, V p2);
    }
}