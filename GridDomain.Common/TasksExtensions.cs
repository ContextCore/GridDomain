using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace GridDomain.Common
{
    public static class TasksExtensions
    {
        public static Task<TResult> ContinueWithSafeResultCast<TResult,TTaskResult>(this Task<TTaskResult> t, Func<TTaskResult,TResult> resultFunc )
        {
            return t.ContinueWith(task =>
            {
                if (!task.IsFaulted) return resultFunc(task.Result);

                var domainException = task.Exception.UnwrapSingle();
                ExceptionDispatchInfo.Capture(domainException).Throw();
                throw new Exception();
            });
        }
    }
}