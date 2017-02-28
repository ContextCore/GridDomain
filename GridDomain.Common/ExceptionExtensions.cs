using System;
using System.Linq;

namespace GridDomain.Common
{
    public static class ExceptionExtensions
    {
        public static Exception UnwrapSingle(this AggregateException aggregateException)
        {
            if (aggregateException == null) return null;

            if (aggregateException.InnerExceptions.Count > 1)
                return aggregateException;

            return aggregateException.InnerExceptions.First()
                                     .UnwrapSingle();
        }

        public static Exception UnwrapSingle(this Exception exeption)
        {
            if (exeption == null) return null;
            var ex = exeption as AggregateException;
            return ex == null ? exeption : ex.UnwrapSingle();
        }
    }
}