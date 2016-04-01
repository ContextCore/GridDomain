using CommonDomain.Persistence;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;

namespace GridDomain.Node.MessageRouteConfigs
{
    //TODO: заменить на регистрацию по договорённости (по интерфейсам IMessageConsumer)
    public class ProcessSkuSaleSaga_RouteConfiguration : IMessageRouteConfiguration
    {
        private readonly IRepository rep;
        private IPublisher _publisher;

        public ProcessSkuSaleSaga_RouteConfiguration(IRepository repository, IPublisher publisher)
        {
            _publisher = publisher;
            rep = repository;
        }

        public void Register(IMessagesRouter messagesRouter)
        {
            //    var sagaRouter = new SagaMessagesRouter<ProcessSkuSaleSaga>(rep,
            //                                                                _publisher, 
            //                                                                messagesRouter);

            //    //TODO: заменить на регистрацию по договорённостии или как-то упростить
            //    sagaRouter.RouteEvent<ShopSalesHistoryBeginEvent,    ProcessSkuSaleSaga>(m => CreateSaga());
            //    sagaRouter.RouteCommand<StartSkuSaleSagaCommand,     ProcessSkuSaleSaga>(m => CreateSaga());
            //    sagaRouter.RouteFailure<CreateSkuSalesNetCommand,
            //                            SkuSalesNetAlreadyExistException,
            //                            ProcessSkuSaleSaga>(m => CreateSaga());

            //    sagaRouter.RouteEvent<SkuSalesNetCreatedEvent,       ProcessSkuSaleSaga>(m => CreateSaga());
            //    sagaRouter.RouteEvent<ShopJoinedSalesNetEvent,       ProcessSkuSaleSaga>(m => CreateSaga());
            //    sagaRouter.RouteEvent<SkuDailySalesRecalulatedEvent, ProcessSkuSaleSaga>(m => CreateSaga());
        }

        //}
        //    return new ProcessSkuSaleSaga(rep);
        //{

        //private ProcessSkuSaleSaga CreateSaga()
    }
}