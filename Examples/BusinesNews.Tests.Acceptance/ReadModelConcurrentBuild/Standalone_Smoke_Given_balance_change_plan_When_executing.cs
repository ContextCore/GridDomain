using System;

namespace BusinesNews.Tests.Acceptance.ReadModelConcurrentBuild
{
    public class Standalone_Smoke_Given_balance_change_plan_When_executing : Standalone_Given_balance_change_plan_When_executing
    {
        protected override TimeSpan Timeout => TimeSpan.FromSeconds(300);
        protected override int BusinessNum => 1;
        protected override int ChangesPerBusiness => 1;
    }
}