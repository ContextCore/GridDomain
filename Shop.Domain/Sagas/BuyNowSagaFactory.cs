using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using Serilog;
using Shop.Domain.Aggregates.UserAggregate.Events;
using Shop.Domain.DomainServices.PriceCalculator;

namespace Shop.Domain.Sagas
{
    public class BuyNowSagaFactory :
        ISagaFactory<ISagaInstance<BuyNow, BuyNowData>, SagaStateAggregate<BuyNowData>>,
        ISagaFactory<ISagaInstance<BuyNow, BuyNowData>, SkuPurchaseOrdered>
    {
        private readonly IPriceCalculator _priceCalculator;
        private readonly ILogger _log;

        public BuyNowSagaFactory(IPriceCalculator priceCalculator, ILogger log)
        {
            _log = log;
            _priceCalculator = priceCalculator;
        }

        public ISagaInstance<BuyNow, BuyNowData> Create(SagaStateAggregate<BuyNowData> message)
        {
            return SagaInstance.New(new BuyNow(_priceCalculator), message, _log);

        }

        public ISagaInstance<BuyNow, BuyNowData> Create(SkuPurchaseOrdered message)
        {
            var sagaState = new BuyNowData(message.SagaId, nameof(BuyNow.Initial));
            var dataAggregate = new SagaStateAggregate<BuyNowData>(sagaState);
            return Create(dataAggregate);
        }
    }
}