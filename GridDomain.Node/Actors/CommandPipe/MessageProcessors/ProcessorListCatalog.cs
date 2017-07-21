namespace GridDomain.Node.Actors.CommandPipe.MessageProcessors
{
    internal class ProcessorListCatalog : ProcessorListCatalogBase<IMessageProcessor>,
                                          IProcessorListCatalog
    {
       
    }
    internal class ProcessorListCatalog<T> : ProcessorListCatalogBase<IMessageProcessor<T>>,
                                             IProcessorListCatalog<T>
    {

    }
}