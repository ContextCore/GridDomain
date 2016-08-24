using GridDomain.EventSourcing;

namespace GridDomain.CQRS.Messaging
{
    /// <summary>
    ///     Обработчик событый домена. Как правило, нужен для сбора модели чтения или оповещений.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEventHandler<in T> : IHandler<T> where T:DomainEvent
    {
    }
}