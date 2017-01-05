using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automatonymous;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using Shop.Domain.Aggregates.AccountAggregate.Commands;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;

namespace Shop.Domain.Sagas
{
    class BuyStockSaga : Saga<BuyStockSagaData>
    {
        public BuyStockSaga()
        {
            Command<ReserveStockCommand>();
            Command<PayForOrderCommand>();
            Command<TakeReservedStockCommand>();
        }

       // public Event<GotTiredEvent> GotTired { get; private set; }
       // public Event<CoffeMadeEvent> CoffeReady { get; private set; }
       // public Event<SleptWellEvent> SleptWell { get; private set; }
       // public Event<Fault<GoSleepCommand>> SleptBad { get; private set; }
       // public Event<CoffeMakeFailedEvent> CoffeNotAvailable { get; private set; }

        public State Reserving { get; private set; }
        public State Paying { get; private set; }
        public State TakingStock { get; private set; }
    }

    internal class BuyStockSagaData : ISagaState
    {
        public string CurrentStateName { get; set; }

    }
}
