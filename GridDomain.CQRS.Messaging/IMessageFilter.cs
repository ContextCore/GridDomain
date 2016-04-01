namespace GridDomain.CQRS.Messaging
{
    /// <summary>
    /// интерфейс фильтрации сообщений из общего потока
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMessageFilter<T>
    {
        bool IsAcceptable(T msg);
    }
}