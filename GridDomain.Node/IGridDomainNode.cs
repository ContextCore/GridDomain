using System;
using System.Linq;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node
{
    //TODO: add execution track status
    public interface IGridDomainNode
    {
        void Execute(params ICommand[] commands);
      
        void ConfirmedExecute(ICommand command, TimeSpan timeout, params ExpectedMessage[] expect);
        void ConfirmedExecute(CommandAndConfirmation commandAnd);
    }
}