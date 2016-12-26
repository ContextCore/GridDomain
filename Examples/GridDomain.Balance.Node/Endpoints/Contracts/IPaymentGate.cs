using System;
using BusinessNews.Domain;
using NMoneys;

namespace BusinessNews.Node.Endpoints.Contracts
{
    /// <summary>
    /// Gate for process payments from customers
    /// </summary>
    public interface IPaymentGate
    {
        void ReplenishAccount(Guid accountId, Money amount, CreditCardInfo creditCard);
    }
}