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
using Serilog.Events;
using Serilog.Filters;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit
{
    public class XUnitAutoTestLoggerConfiguration : DefaultLoggerConfiguration
    {
        public XUnitAutoTestLoggerConfiguration(ITestOutputHelper output, LogEventLevel level = LogEventLevel.Verbose, string logFileName = null):base(level, logFileName)
        {
            this.XUnit(level,output)
            .Filter.ByExcluding(Matching.WithProperty<string>("Source", s => s.Contains("Akka.Cluster")))
            .Filter.ByExcluding(Matching.WithProperty<string>("Class", s => s.Contains("ClusterSingletonManager")))
            .Filter.ByExcluding(Matching.WithProperty<string>("Class", s => s.Contains("ClusterCoreDaemon")))
            .Filter.ByExcluding(Matching.WithProperty<string>("Class", s => s.Contains("CoordinatedShutdown")))
        }

        
    }
    
     
    
}