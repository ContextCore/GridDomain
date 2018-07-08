using System;
using System.Linq;

namespace GridDomain.Common
{
    public static class ExceptionExtensions
    {
        public static Exception UnwrapSingle(this AggregateException aggregateException)
        {
            if (aggregateException == null)
                return null;

            if (aggregateException.InnerExceptions.Count > 1)
                return aggregateException;

            if(aggregateException.InnerExceptions.Count == 0)
            {
                //for cases when inner exceptions were lost due to hyperion serializer
                return aggregateException.InnerException ?? aggregateException;
            }
            //for cases when inner exceptions were lost due to hyperion serializer
            if (aggregateException.InnerException != null)
                return aggregateException.InnerException;

            return aggregateException.InnerExceptions.First().UnwrapSingle();
        }

        public static Exception UnwrapSingle(this Exception exeption)
        {
            if (exeption == null)
                return null;
            return !(exeption is AggregateException aggregateException) ? exeption : aggregateException.InnerException.UnwrapSingle();
        }
    }
}