using System;
using System.Threading.Tasks;
using GridDomain.Common;
using NUnit.Framework;

namespace GridDomain.Tests.Framework
{
    public static class NUnitAssertEx
    {
        public static async Task ShouldThrow<T>(this Task task, Predicate<T> predicate = null ) where T : Exception
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                var exception = ex.UnwrapSingle();
                Assert.IsInstanceOf<T>(exception);
                if (predicate == null)
                    Assert.Pass();

                if (predicate != null && predicate((T)exception))
                    Assert.Pass();
                 
                Assert.Fail($"{typeof(T).Name} was raised but did not satisfy predicate");
            }
            Assert.Fail($"{typeof(T).Name} was not raised");
        }
    }
}