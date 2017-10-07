using Serilog;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.NodeCommandExecution {
    class SerilogTestOutput : ITestOutputHelper
    {

        public void WriteLine(string message)
        {
            Log.Logger.Information(message);
        }

        public void WriteLine(string format, params object[] args)
        {
            Log.Logger.Information(format,args);
        }
    }
}