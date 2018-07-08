using System;
using GridDomain.Common;
using GridDomain.ProcessManagers;

namespace GridDomain.Node.Actors.ProcessManagers.Messages
{
    public class GetProcessState
    {
        public string Id { get; }

        public GetProcessState(string id)
        {
            Id = id;
        }
    }
}