using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace GridDomain.Common
{
    public static class TasksExtensions
    {
        public static void TransparentThrowOnException(this Task t)
        {
            if (!t.IsFaulted) return;
            var domainException = t.Exception.UnwrapSingle();
            ExceptionDispatchInfo.Capture(domainException).Throw();
        }

        public static Task<TResult> ContinueWithSafeResultCast<TResult,TTaskResult>(this Task<TTaskResult> t, Func<TTaskResult,TResult> resultFunc )
        {
            return t.ContinueWith(task =>
            {
                task.TransparentThrowOnException();
                return resultFunc(task.Result);
            });
        }
    }
}