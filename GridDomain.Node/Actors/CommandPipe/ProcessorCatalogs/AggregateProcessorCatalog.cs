using System.Linq;
using GridDomain.CQRS;

namespace GridDomain.Node.Actors
{
    class AggregateProcessorCatalog : TypeCatalog<Processor,ICommand>, IAggregateProcessorCatalog
    {
        public Processor GetAggregateProcessor(ICommand command)
        {
            return GetProcessor(command);
        }

        public override void Add<U>(Processor processor)
        {
            _catalog[typeof(U)] = processor;
        }
    }

    //class ProcessorListCatalog<T>
    //{
    //    private readonly IDictionary<Type,List<Processor>> _catalog = new Dictionary<Type, List<Processor>>();
    //
    //    public void Add<U>(Processor processor) where U:T
    //    {
    //        List<Processor> list;
    //        var messageType = typeof(U);
    //        if (!_catalog.TryGetValue(messageType, out list))
    //            list = _catalog[messageType] = new List<Processor>();
    //        list.Add(processor);
    //    }
    //
    //    protected IReadOnlyCollection<Processor> GetProcessor(T message)
    //    {
    //        var messageType = message.GetType();
    //        List<Processor> processor;
    //        _catalog.TryGetValue(messageType, out processor);
    //        return processor;
    //    }
    //}
}