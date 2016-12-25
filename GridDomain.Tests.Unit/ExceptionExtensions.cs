using System;

namespace GridDomain.Tests.Unit
{
    public static class ExceptionExtensions
    {
        public static Exception UnwrapInner(this Exception exception)
        {
            return exception.InnerException == null ? exception : UnwrapInner(exception.InnerException);
        }
    }
}