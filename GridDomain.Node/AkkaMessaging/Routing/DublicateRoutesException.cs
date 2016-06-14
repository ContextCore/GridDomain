using System;
using System.Collections.Generic;

namespace GridDomain.Node.AkkaMessaging.Routing
{
    internal class DublicateRoutesException : Exception
    {
        public DublicateRoutesException(IEnumerable<string> dublicateRoutes)
            :base($"Found dublicate routes for messages: {string.Join(";",dublicateRoutes)}")
        {
        }
    }
}