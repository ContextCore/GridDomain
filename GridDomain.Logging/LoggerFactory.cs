using System.Diagnostics;

namespace GridDomain.Logging
{
    public abstract class LoggerFactory
    {
        public abstract ILogger GetLogger(string className = null);
        protected string GetClassName()
        {
            StackFrame frame = new StackFrame(3, false);
            var declaringType = frame.GetMethod().DeclaringType;
            return declaringType?.Name ?? "unknown";
        }
    }
}