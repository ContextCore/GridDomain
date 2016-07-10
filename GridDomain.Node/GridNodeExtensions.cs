using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node
{
    public static class GridNodeExtensions
    {
        public static Task<object> Execute(this IGridDomainNode node, CommandAndConfirmation data)
        {
            return node.Execute(data.Command, data.ExpectedMessages);
        }

        public static Task<TMessage> Execute<TMessage>(this IGridDomainNode node, 
                                                       ICommand command,
                                                       ExpectedMessage<TMessage> expect)
        {
            return Execute(node, new CommandAndConfirmation(command, expect))
                    .ContinueWith(t => (TMessage)t.Result);
        }
        public static object Execute(this IGridDomainNode node, CommandAndConfirmation command, TimeSpan timeout)
        {
            var commandExecutionTask = node.Execute(command);
            if (!commandExecutionTask.Wait(timeout))
                throw new TimeoutException($"Command execution timed out");
            return commandExecutionTask.Result;
        }

        public static T Execute<T>(this IGridDomainNode node, ICommand command, TimeSpan timeout,ExpectedMessage<T> expect)
        {
            var commandExecutionTask = node.Execute(command, expect);
            if (!commandExecutionTask.Wait(timeout))
                throw new TimeoutException($"Command execution timed out");
            return (T)commandExecutionTask.Result;
        }

        public static void Execute(this IGridDomainNode node, ICommand command, TimeSpan timeout, ExpectedMessage[] expect)
        {
            Execute(node, new CommandAndConfirmation(command, expect), timeout);
        }
    }
}