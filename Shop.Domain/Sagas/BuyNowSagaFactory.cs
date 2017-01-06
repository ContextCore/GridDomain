using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using Shop.Domain.Aggregates.UserAggregate.Events;
using Shop.Domain.DomainServices;

namespace Shop.Domain.Sagas
{
    public class BuyNowSagaFactory :
        ISagaFactory<ISagaInstance<BuyNowSaga, BuyNowSagaData>, SagaDataAggregate<BuyNowSagaData>>,
        ISagaFactory<ISagaInstance<BuyNowSaga, BuyNowSagaData>, SkuPurchaseOrdered>
    {
        private readonly IPriceCalculator _priceCalculator;

        public BuyNowSagaFactory(IPriceCalculator priceCalculator)
        {
            _priceCalculator = priceCalculator;
        }

        public ISagaInstance<BuyNowSaga, BuyNowSagaData> Create(SagaDataAggregate<BuyNowSagaData> message)
        {
            return SagaInstance.New(new BuyNowSaga(_priceCalculator), message);

        }

        public ISagaInstance<BuyNowSaga, BuyNowSagaData> Create(SkuPurchaseOrdered message)
        {
            var sagaState = new BuyNowSagaData(nameof(BuyNowSaga.ReceivingPurchaseOrder));
            var dataAggregate = new SagaDataAggregate<BuyNowSagaData>(message.SagaId,sagaState);
            return Create(dataAggregate);
        }
    }
}