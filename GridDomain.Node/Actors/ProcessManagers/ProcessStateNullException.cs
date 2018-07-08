using System;

namespace GridDomain.Node.Actors.ProcessManagers {
    internal class ProcessStateNullException : Exception
    {
        public ProcessStateNullException() : base("Process state, produced by state factory is null, check factory for possible errors") { }
    }
}