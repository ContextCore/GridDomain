namespace GridDomain.Scenarios {
    public static class AggregateScenarioExtensions
    {
        public static IAggregateScenarioRunBuilder Run(this IAggregateScenario scenario)
        {
            return new AggregateScenarioRunBuilder(scenario);
        }
    }
}