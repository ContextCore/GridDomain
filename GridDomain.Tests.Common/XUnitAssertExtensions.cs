using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors.Aggregates.Exceptions;
using Xunit;

namespace GridDomain.Tests.Common
{
    public static class XUnitAssertExtensions
    {
        public static async Task<TEx> CommandShouldThrow<TEx>(this Task t, Predicate<TEx> predicate = null) where TEx : Exception
        {
            var commandException = await t.ShouldThrow<CommandExecutionFailedException>();
            var exception = commandException.InnerException.UnwrapSingle();
            Assert.IsAssignableFrom<TEx>(exception);

            if(predicate != null && !predicate((TEx)exception))
                throw new InvalidExceptionReceivedException();

            return (TEx)exception;
        }

        public static async Task<TEx> ShouldThrow<TEx>(this Task t, Predicate<TEx> predicate = null) where TEx : Exception
        {
            try
            {
                await t;
            }
            catch (Exception ex)
            {
                var exception = ex.UnwrapSingle();
                Assert.IsAssignableFrom<TEx>(exception);

                if (predicate != null && !predicate((TEx) exception))
                    throw new InvalidExceptionReceivedException();

                return (TEx) exception;
            }

            Assert.False(true, "No exception was raised");
            throw new Exception();
        }

        public class InvalidExceptionReceivedException : Exception {}
    }
}