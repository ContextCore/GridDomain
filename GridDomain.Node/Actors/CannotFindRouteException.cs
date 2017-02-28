using System;

namespace GridDomain.Node.Actors
{
    public class CannotFindRouteException : Exception
    {
        public CannotFindRouteException(object msg)
        {
            Msg = msg;
        }

        public object Msg { get; }
    }
}