using System;
using System.Diagnostics.Tracing;
using System.Linq;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.Aggregates.Messages;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.ProcessManagers;
using GridDomain.Node.Actors.ProcessManagers.Messages;
using GridDomain.Tests.Common;
using NMoneys;
using Serilog;
using Serilog.Events;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit
{
    public class XUnitAutoTestLoggerConfiguration : DefaultLoggerConfiguration
    {
        public XUnitAutoTestLoggerConfiguration(ITestOutputHelper output, LogEventLevel level = LogEventLevel.Verbose, string logFileName = null):base(level, logFileName)
        {
            //dont want overload CI console output with logs 
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI")))
                level = LogEventLevel.Warning;
            
            WriteTo.XunitTestOutput(output,level,DefaultTemplate);
        }
    }
}