using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using Serilog;
using Shop.Domain.Aggregates.UserAggregate.Events;
using Shop.Domain.DomainServices.PriceCalculator;

namespace Shop.Domain.Sagas
{
    public class BuyNowSagaFactory : ISagaFactory<ISaga<BuyNow, BuyNowData>, SagaStateAggregate<BuyNowData>>,
                                     ISagaFactory<ISaga<BuyNow, BuyNowData>, SkuPurchaseOrdered>
    {
        private readonly ILogger _log;
        private readonly IPriceCalculator _priceCalculator;

        public BuyNowSagaFactory(IPriceCalculator priceCalculator, ILogger log)
        {
            _log = log;
            _priceCalculator = priceCalculator;
        }

        public ISaga<BuyNow, BuyNowData> Create(SagaStateAggregate<BuyNowData> message)
        {
            return Saga.New(new BuyNow(_priceCalculator), message, _log);
        }

        public ISaga<BuyNow, BuyNowData> Create(SkuPurchaseOrdered message)
        {
            var sagaState = new BuyNowData(message.SagaId, nameof(BuyNow.Initial));
            var dataAggregate = new SagaStateAggregate<BuyNowData>(sagaState);
            return Create(dataAggregate);
        }
    }
}