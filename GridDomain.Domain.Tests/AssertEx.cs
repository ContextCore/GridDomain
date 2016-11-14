using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.Logging;
using NUnit.Framework;

namespace GridDomain.Tests
{
    public static class AssertEx
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
                if (predicate == null) return;

                if(predicate((T)exception))
                    Assert.Pass();
                else 
                    Assert.Fail($"{typeof(T).Name} was raised but did not satisfy predicate");
                return;
            }
            Assert.Fail($"{typeof(T).Name} was not raised");
        }
    }
}