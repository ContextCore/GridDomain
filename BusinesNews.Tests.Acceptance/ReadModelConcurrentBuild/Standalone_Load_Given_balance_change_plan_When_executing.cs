namespace BusinesNews.Tests.Acceptance.ReadModelConcurrentBuild
{
    public class Standalone_Load_Given_balance_change_plan_When_executing : Standalone_Given_balance_change_plan_When_executing
    {
        protected override int BusinessNum => 10;
        protected override int ChangesPerBusiness => 10;
    }
}