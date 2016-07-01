using System;

namespace GridDomain.Logging
{
    public interface ISoloLogger
    {
        ISoloLogger ForContext(string name, object value);
        void Trace(string message);
        void Debug(string message, params object[] parameters);
        void Info(string message, params object[] parameters);
        void Error(Exception ex, string message = null, params object[] parameters);
        void Warn(Exception ex, string message, params object[] parameters);
        void Warn(string message, params object[] parameters);
    }
}