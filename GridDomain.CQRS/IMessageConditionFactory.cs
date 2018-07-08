namespace GridDomain.CQRS {
    public interface IMessageConditionFactory<out T, out TBuilder>: IMessageFilter<TBuilder>
    {
        T Create();
    }
}