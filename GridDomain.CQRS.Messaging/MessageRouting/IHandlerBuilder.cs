using System.Threading.Tasks;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IHandlerBuilder<TMessage, THandler> where THandler : IHandler<TMessage>
    {
        Task Register();
        IHandlerBuilder<TMessage, THandler> WithCorrelation(string propertyName);
    }
}