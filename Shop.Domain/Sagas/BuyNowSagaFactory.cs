using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using Serilog;
using Shop.Domain.Aggregates.UserAggregate.Events;
using Shop.Domain.DomainServices.PriceCalculator;

namespace Shop.Domain.Sagas
{
    public class BuyNowSagaFactory : ISagaFactory<ISaga<BuyNowData>, SagaStateAggregate<BuyNowData>>,
                                     ISagaFactory<ISaga<BuyNowData>, SkuPurchaseOrdered>
    {
        private readonly ILogger _log;
        private readonly IPriceCalculator _priceCalculator;

        public BuyNowSagaFactory(IPriceCalculator priceCalculator, ILogger log)
        {
            _log = log;
            _priceCalculator = priceCalculator;
        }

        public ISaga<BuyNowData> Create(SagaStateAggregate<BuyNowData> message)
        {
            return new Saga<BuyNowData>(new BuyNow(_priceCalculator),message.Data, _log);
        }

        public ISaga<BuyNowData> Create(SkuPurchaseOrdered message)
        {
            var sagaState = new BuyNowData(message.SagaId, nameof(BuyNow.Initial));
            var dataAggregate = new SagaStateAggregate<BuyNowData>(sagaState);
            return Create(dataAggregate);
        }
    }
}