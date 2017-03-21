using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using Serilog;
using Shop.Domain.Aggregates.UserAggregate.Events;
using Shop.Domain.DomainServices.PriceCalculator;

namespace Shop.Domain.Sagas
{
    public class BuyNowSagaFactory : ISagaFactory<ISaga<BuyNowState>, BuyNowState>,
                                     ISagaFactory<ISaga<BuyNowState>, SkuPurchaseOrdered>
    {
        private readonly ILogger _log;
        private readonly IPriceCalculator _priceCalculator;

        public BuyNowSagaFactory(IPriceCalculator priceCalculator, ILogger log)
        {
            _log = log;
            _priceCalculator = priceCalculator;
        }

        public ISaga<BuyNowState> Create(BuyNowState state)
        {
            return new Saga<BuyNowState>(new BuyNow(_priceCalculator),state, _log);
        }

        public ISaga<BuyNowState> Create(SkuPurchaseOrdered message)
        {
            return Create(new BuyNowState(message.SagaId, nameof(BuyNow.Initial)));
        }
    }
}