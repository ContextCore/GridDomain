using System;
using System.Linq;
using GridDomain.Common;
using GridDomain.Logging;
using NUnit.Framework;

namespace GridDomain.Tests
{
    public static class AssertEx
    {
        public static void ThrowsInner<T>(Action act) where T : Exception
        {
            try
            {
                act.Invoke();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<T>(ex.UnwrapSingle());
                return;
            }
            Assert.Fail($"{typeof(T).Name} was not raised");
        }
    }
}