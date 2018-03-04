using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.EventSourcing;
using Xunit;

namespace GridDomain.Tests.Common
{
    public static class XUnitAssertExtensions
    {
        
        public static async Task<TEx> CommandShouldThrow<TEx>(this Task t, Predicate<TEx> predicate = null) where TEx : Exception
        {
            //return t.ShouldThrow(predicate);
            var commandException = await t.ShouldThrow<CommandExecutionFailedException>();
            var exception = commandException.InnerException.UnwrapSingle();
            return CheckException(exception, predicate);
        }
        
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

            Assert.False(true, "No exception was raised");
            throw new Exception();
        }

        private static TEx CheckException<TEx>(Exception ex, Predicate<TEx> predicate = null) where TEx : Exception
        {
            var exception = ex.UnwrapSingle();
            Assert.IsAssignableFrom<TEx>(exception);

            if (predicate != null && !predicate((TEx) exception))
                throw new PredicateNotMatchException();
            return (TEx)exception;
        }

        public class PredicateNotMatchException : Exception {}
    }
}