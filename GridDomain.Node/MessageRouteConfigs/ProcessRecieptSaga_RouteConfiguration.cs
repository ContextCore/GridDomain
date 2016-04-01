using CommonDomain.Persistence;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Node.MessageRouteConfigs
{
    //TODO: перейти на регистрацию по договорёности по IHandler<T>? 
    internal class ProcessRecieptSaga_RouteConfiguration : IMessageRouteConfiguration
    {
        private readonly IRepository rep;
        private IPublisher _publisher;

        public ProcessRecieptSaga_RouteConfiguration(IRepository repository, IPublisher publisher)
        {
            _publisher = publisher;
            rep = repository;
        }

        public void Register(IMessagesRouter messagesRouter)
        {
            //    var sagaRouter = new SagaMessagesRouter<ProcessRecieptSaga>(rep,
            //                                                                _publisher, messagesRouter);

            //    sagaRouter.RouteCommand<ProcessNewRecieptCommand, ProcessRecieptSaga>(cmd => new ProcessRecieptSaga(rep));
            //    sagaRouter.RouteEvent<ProcessSkuSalesSagaData_StateChanged, ProcessRecieptSaga>( cmd => new ProcessRecieptSaga(rep));
        }
    }
}