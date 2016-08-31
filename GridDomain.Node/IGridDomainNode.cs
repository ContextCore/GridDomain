using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node
{
    public interface IGridDomainNode : ICommandExecutor
    {
    }

 
}