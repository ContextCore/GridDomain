using GridDomain.Balance.Domain.BalanceAggregate;
using GridDomain.Balance.Domain.BalanceAggregate.Commands;
using GridDomain.CQRS;

namespace GridDomain.Node.AkkaMessaging
{
    public class BalanceHostActor : AggregateHostActor<MoneyBalance, BalanceActor>,
        IHandler<CreateBalanceCommand>,
        IHandler<WithdrawalBalanceCommand>,
        IHandler<ReplenishBalanceCommand>
    {
        //public BalanceHostActor()
        //{
        //    RegisterCorrelation<CreateBalanceCommand>(c => c.BalanceId);
        //    RegisterCorrelation<WithdrawalBalanceCommand>(c => c.BalanceId);
        //    RegisterCorrelation<ReplenishBalanceCommand>(c => c.BalanceId);
        //}
        public void Handle(CreateBalanceCommand e)
        {
            TellTo(e, e.BalanceId);
        }

        public void Handle(WithdrawalBalanceCommand e)
        {
            TellTo(e, e.BalanceId);
        }

        public void Handle(ReplenishBalanceCommand e)
        {
            TellTo(e, e.BalanceId);
        }
    }
}