namespace GridDomain.EventSourcing.Adapters
{
    /// <summary>
    /// Object adapter used for value object upgrade in domain events
    /// If value object is changed, you should create one object adaptor for this
    /// and none of domain event adapter.
    /// Works as part of deserialiation logic.
    /// Can be used on interface implementation or child classes change  
    /// </summary>
    /// <typeparam name="TFrom"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    public interface IObjectAdapter<TFrom, TTo> : IObjectAdapter
    {
        TTo Convert(TFrom evt);
    }
}