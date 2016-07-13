using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace GridDomain.Node
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


    public static class ExceptionExtensions
    {
        public static Exception UnwrapSingle(this AggregateException aggregateException)
        {
            if (aggregateException == null) return null;

            if (aggregateException.InnerExceptions.Count > 1)
                return aggregateException;
            return aggregateException.InnerExceptions.First();
        }

        public static Exception UnwrapSingle(this Exception exeption)
        {
            if (exeption == null) return null;
            AggregateException ex = exeption as AggregateException;
            return ex == null ? exeption : ex.UnwrapSingle();
        }
    }
}