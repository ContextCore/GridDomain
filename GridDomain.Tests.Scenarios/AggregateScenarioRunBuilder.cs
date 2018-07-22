namespace GridDomain.Tests.Scenarios {

    public class AggregateScenarioRunBuilder: IAggregateScenarioRunBuilder
    {
        public AggregateScenarioRunBuilder(IAggregateScenario builder)
        {
            Scenario = builder;
        }
        public IAggregateScenario Scenario { get; }
    }
}