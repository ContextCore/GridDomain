using System;

namespace GridDomain.EventSourcing.Sagas
{
    /// <summary>
    ///     Provides any custom logic for empty saga creation
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public interface IEmptySagaFactory<TSaga> where TSaga : ISaga
    {
        TSaga Create(Guid id);
    }
}