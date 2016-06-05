using System;

namespace BusinessNews.Domain
{
    public interface IPaymentGate
    {
        void ReplenishAccount(Guid accountId, CreditCardInfo amount);
    }
}