namespace GridDomain.Tests.Acceptance
{
    class ChangeSubscriptionCommand
    {
        private NotEnoughFondsFailure e;

        public ChangeSubscriptionCommand(NotEnoughFondsFailure e)
        {
            this.e = e;
        }
    }
}