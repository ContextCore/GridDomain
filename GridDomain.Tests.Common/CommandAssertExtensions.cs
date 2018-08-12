using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Common
{
    public static class XUnitAssertExtensions
    {
        public static async Task<TEx> ShouldThrow<TEx>(this Task t, Predicate<TEx> predicate = null) where TEx : Exception
        {
            try
            {
                await t;
            }
            catch (Exception ex)
            {
                return CheckException(ex, predicate);
            }

            throw new ExpectedExceptionWasNotRaisedException();
        }

        public class ExpectedExceptionWasNotRaisedException : Exception { }

        private static TEx CheckException<TEx>(Exception ex, Predicate<TEx> predicate = null) where TEx : Exception
        {
            var exception = ex.UnwrapSingle();
            if (!(exception is TEx))
                throw new ExpectedExceptionWasNotRaisedException();

            if (predicate != null && !predicate((TEx) exception))
                throw new PredicateNotMatchException();
            return (TEx)exception;
        }

        public class PredicateNotMatchException : Exception {}
    }
}