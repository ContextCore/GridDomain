namespace GridDomain.Query
{
    public interface ISingleQuery<T> : IGenericQuery<T> {}
    
    public interface ISingleQuery<TParam, TReturn> : IGenericQuery<TParam, TReturn> {}

    public interface ISingleQuery<TParam1, TParam2, TReturn> : IGenericQuery<TParam1, TParam2, TReturn> {}

}