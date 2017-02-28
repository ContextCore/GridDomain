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

            var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token))
                                          .ConfigureAwait(false);
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

            var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token))
                                          .ConfigureAwait(false);
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

        public static Task ToChain(this IEnumerable<Task> tasks)
        {
            return tasks.Aggregate<Task, Task>(null, (current, task) => current?.ContinueWith(t => task) ?? task);
        }

        public static Task<IEnumerable<T>> ToChain<T>(this IEnumerable<Task<IEnumerable<T>>> tasks)
        {
            return tasks.Aggregate<Task<IEnumerable<T>>, Task<IEnumerable<T>>>(null,
                (current, task) => current?.ContinueWith(t => task.Result.Union(t.Result)) ?? task);
        }
    }
}