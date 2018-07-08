using System;
using GridDomain.Node;
using GridDomain.Tests.Common;
using Serilog;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit {
    public static class LoggerConfigurationExtensions
    {

        public static LoggerConfiguration XUnit(this LoggerConfiguration cfg, LogEventLevel level, ITestOutputHelper output)
        {
            //dont want overload CI console output with logs 
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI")))
                level = LogEventLevel.Warning;

            var template = level.GetTemplate();
            return cfg.WriteTo.XunitTestOutput(output, level, template);
        }
    }
}