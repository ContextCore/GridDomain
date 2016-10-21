using System;
using GridDomain.Common;
using NUnit.Framework;

namespace GridDomain.Tests.MessageWaiting.Commanding
{
    public static class ExceptionExtensions
    {
        public static void AssertInner<T>(this Exception e) where T: Exception
        {
            Assert.IsInstanceOf<T>(e.UnwrapSingle());
        }
    }
}