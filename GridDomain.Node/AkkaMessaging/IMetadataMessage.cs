using System.Collections.Generic;

namespace GridDomain.Node.AkkaMessaging
{
    public interface IMetadataMessage
    {
        IList<MetadataEntry> Metadata { get; set; }
    }
}