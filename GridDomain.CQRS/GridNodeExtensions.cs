using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using GridDomain.Common;

namespace GridDomain.CQRS
{

    [Obsolete("Use GridNodeCommandingExtensions instead")]
    public static class GridNodeExtensions
    {
        public static Task<T> Execute<T>(this ICommandExecutor node, CommandPlan data)
        {
            return node.Execute(data.Command, data.ExpectedMessages)
                       .ContinueWithSafeResultCast(result => (T)result);
        }
        public static Task<T> Execute<T>(this ICommandExecutor node, ICommand command, params ExpectedMessage[] expectedMessage)
        {
            return node.Execute(command, expectedMessage)
                       .ContinueWithSafeResultCast(result => (T)result);
        }

        public static Task<T> Execute<T>(this ICommandExecutor node, ICommand command, ExpectedMessage<T> expectedMessage)
        {
            return Execute<T>(node, new CommandPlan(command, expectedMessage));
        }

        public static T Execute<T>(this ICommandExecutor node, ICommand command, TimeSpan timeout, ExpectedMessage<T> expectedMessage)
        {
            return Execute<T>(node, new CommandPlan(command, expectedMessage), timeout);
        }

        public static T Execute<T>(this ICommandExecutor node, CommandPlan command, TimeSpan timeout)
        {
            var commandExecutionTask = node.Execute<T>(command);
            try
            {
                if (!commandExecutionTask.Wait(timeout))
                    throw new TimeoutException("Command execution timed out");

                return commandExecutionTask.Result;
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.UnwrapSingle()).Throw();
            }

            return commandExecutionTask.Result;
        }

        public static T Execute<T>(this ICommandExecutor node, ICommand command, TimeSpan timeout, params ExpectedMessage[] expectedMessage)
        {
            return (T)node.Execute(command, timeout, expectedMessage);
        }

        public static object Execute(this ICommandExecutor node, CommandPlan command, TimeSpan timeout)

        {
            return Execute<object>(node, command, timeout);
        }


        public static object Execute(this ICommandExecutor node, ICommand command, TimeSpan timeout, params ExpectedMessage[] expectedMessage)
        {
            return Execute(node, new CommandPlan(command, timeout, expectedMessage),timeout);
        }
    }
}