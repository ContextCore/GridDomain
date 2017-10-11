using System;
using GridDomain.Common;
using GridDomain.ProcessManagers;

namespace GridDomain.Node.Actors.ProcessManagers.Messages
{
    class GetProcessState
    {
        public Guid Id { get; }

        public GetProcessState(Guid id)
        {
            Id = id;
        }

    }
}