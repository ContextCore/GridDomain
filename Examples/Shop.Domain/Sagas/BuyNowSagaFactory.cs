using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using Serilog;
using Shop.Domain.Aggregates.UserAggregate.Events;
using Shop.Domain.DomainServices.PriceCalculator;

namespace Shop.Domain.Sagas
{
    public class BuyNowSagaFactory : ISagaCreator<BuyNowState>,
                                     ISagaCreator<BuyNowState,SkuPurchaseOrdered>
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

        public ISaga<BuyNowState> CreateNew(SkuPurchaseOrdered message, Guid? sagaid = null)
        {
            return Create(new BuyNowState(sagaid ?? Guid.NewGuid(), nameof(BuyNow.Initial)));
        }
    }
}