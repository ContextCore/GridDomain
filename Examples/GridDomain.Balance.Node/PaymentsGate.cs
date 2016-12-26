using System;
using BusinessNews.Domain;
using BusinessNews.Domain.AccountAggregate.Commands;
using BusinessNews.Node.Endpoints.Contracts;
using GridDomain.Node;
using NMoneys;

namespace BusinessNews.Node
{
    public class PaymentsGate : IPaymentGate
    {
        private readonly IGridDomainNode _node;

        public PaymentsGate(IGridDomainNode node)
        {
            _node = node;
        }

        //client gets accountId from other endpoints, it is included in business view models
        public void ReplenishAccount(Guid accountId, Money amount, CreditCardInfo creditCardInfo)
        {
            _node.Execute(new ReplenishAccountByCardCommand(accountId, amount,creditCardInfo));
        }
    }
}