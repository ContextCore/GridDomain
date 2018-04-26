namespace GridDomain.CQRS
{
    public interface IMessageWaiter<T> : IExpectationBuilder<IConditionFactory<T>> {}
}