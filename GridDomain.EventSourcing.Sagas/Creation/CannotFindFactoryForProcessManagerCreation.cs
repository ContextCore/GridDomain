using System;

namespace GridDomain.Processes.Creation
{
    public class CannotFindFactoryForProcessManagerCreation : Exception
    {
        public CannotFindFactoryForProcessManagerCreation(Type process, object msg)
        {
            Process = process;
            Msg = msg;
        }

        public Type Process { get; }
        public object Msg { get;  }
    }
}