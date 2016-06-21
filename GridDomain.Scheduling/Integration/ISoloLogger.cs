using System;

namespace GridDomain.Scheduling.Integration
{
    public interface ISoloLogger
    {
        void Debug(string message);
        void Info(string message);
        void Error(Exception error, string message);
    }
}