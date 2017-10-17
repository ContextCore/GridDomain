using System;

namespace GridDomain.Node.Actors.ProcessManagers {
    internal class CannotFindProcessStatePersistenceActor : Exception
    {
        public string ProbedPath { get; }

        public CannotFindProcessStatePersistenceActor(string probedPath)
        {
            ProbedPath = probedPath;
        }
    }
}