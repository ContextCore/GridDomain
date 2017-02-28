namespace GridDomain.Common
{
    public interface ICatalog<out TData, in TMessage>
    {
        /// <summary>
        ///     Returns empty list if no processor was found
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        TData Get(TMessage evt);
    }
}