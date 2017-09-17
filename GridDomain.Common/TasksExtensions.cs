using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GridDomain.Common
{
    public static class TasksExtensions
    {
        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task,
                                                                TimeSpan timeout,
                                                                string message = null)
        {
            var timeoutCancellationTokenSource = new CancellationTokenSource();

            var completedTask =
                await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token)).ConfigureAwait(false);
            if (completedTask == task)
            {
                timeoutCancellationTokenSource.Cancel();
                return await task; // Very important in order to propagate exceptions
            }
            throw new TimeoutException(message ?? "The operation has timed out.");
        }

        public static async Task TimeoutAfter(this Task task, TimeSpan timeout, string message = null)
        {
            var timeoutCancellationTokenSource = new CancellationTokenSource();

            var completedTask =
                await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token)).ConfigureAwait(false);
            if (completedTask == task)
            {
                timeoutCancellationTokenSource.Cancel();
                await task; // Very important in order to propagate exceptions
            }
            else
            {
                throw new TimeoutException(message ?? "The operation has timed out.");
            }
        }

        public static async Task ToChain(this IEnumerable<Task> tasks)
        {
            foreach (var t in tasks)
                await t;
        }

      
    }
}