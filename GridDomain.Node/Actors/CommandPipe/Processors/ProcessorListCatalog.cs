namespace GridDomain.Node.Actors.CommandPipe.Processors
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