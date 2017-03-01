using System;
using System.Threading.Tasks;
using GridDomain.Common;
using Xunit;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
    public static class XUnitAssertExtensions
    {
        public static async Task<TEx> ShouldThrow<TEx>(this Task t, Predicate<TEx> predicate = null) where TEx : Exception
        {
            return await Assert.ThrowsAsync<TEx>(async () =>
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
                                                         throw exception;
                                                     }
                                                 });
        }

        public class InvalidExceptionReceivedException : Exception {}
    }
}