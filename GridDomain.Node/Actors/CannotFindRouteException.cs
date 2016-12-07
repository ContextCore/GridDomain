using System;

namespace GridDomain.Node.Actors
{
    public class CannotFindRouteException : Exception
    {
        public object Msg { get; }

        public CannotFindRouteException(object msg)
        {
            Msg = msg;
        }
    }
}