namespace GridDomain.Tests.Acceptance
{
    class PayForSubscriptionCommand
    {
        private SubscriptionExpiredEvent e;

        public PayForSubscriptionCommand(SubscriptionExpiredEvent e)
        {
            this.e = e;
        }
    }
}