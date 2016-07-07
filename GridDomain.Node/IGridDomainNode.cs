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
      
        void ConfirmedExecute(ICommand command, ExpectedMessage expect,TimeSpan timeout);
        void ConfirmedExecute(CommandAndConfirmation commandAnd);
    }

    //public static class GridDomainNodeExtensions
    //{
    //    public static void ConfirmedExecute(this IGridDomainNode node, CommandAndConfirmation commandAndExpectedMessage)
    //    {
    //        node.ConfirmedExecute(commandAndExpectedMessage.Command,commandAndExpectedMessage.ExpectedMessage, commandAndExpectedMessage.Timeout);
    //    }
    //}
}