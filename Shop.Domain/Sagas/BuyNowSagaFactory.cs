using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using Shop.Domain.Aggregates.UserAggregate.Events;
using Shop.Domain.DomainServices.PriceCalculator;

namespace Shop.Domain.Sagas
{
    public class BuyNowSagaFactory :
        ISagaFactory<ISagaInstance<BuyNow, BuyNowData>, SagaDataAggregate<BuyNowData>>,
        ISagaFactory<ISagaInstance<BuyNow, BuyNowData>, SkuPurchaseOrdered>
    {
        private readonly IPriceCalculator _priceCalculator;

        public BuyNowSagaFactory(IPriceCalculator priceCalculator)
        {
            _priceCalculator = priceCalculator;
        }

        public ISagaInstance<BuyNow, BuyNowData> Create(SagaDataAggregate<BuyNowData> message)
        {
            return SagaInstance.New(new BuyNow(_priceCalculator), message);

        }

        public ISagaInstance<BuyNow, BuyNowData> Create(SkuPurchaseOrdered message)
        {
            var sagaState = new BuyNowData(nameof(BuyNow.Initial));
            var dataAggregate = new SagaDataAggregate<BuyNowData>(message.SagaId,sagaState);
            return Create(dataAggregate);
        }
    }
}