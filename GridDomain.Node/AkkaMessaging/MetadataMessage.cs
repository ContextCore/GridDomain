using System;
using System.Collections.Generic;

namespace GridDomain.Node.AkkaMessaging
{
    [Serializable]
    public class MetadataMessage : IMetadataMessage
    {
        public IList<MetadataEntry> Metadata { get; set; }
    }
}