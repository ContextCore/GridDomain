using System;
using System.Threading.Tasks;
using Akka.TestKit.Xunit2;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit
{
    public class NodeTestKit : TestKit
    {
        protected readonly NodeConfiguration NodeConfig;

        protected NodeTestKit(ITestOutputHelper output, NodeTestFixture fixture) : base(fixture.SystemConfigFactory(), fixture.Name)
        {
           // output.WriteLine($"before test {GetType().Name}");
          //  DumpMemoryUsage(output);

            Fixture = fixture;
            Fixture.ActorSystemCreator = () => Sys;
            Fixture.Output = output;
            NodeConfig = fixture.NodeConfig;

            Node = fixture.CreateNode().Result;
        }

        protected GridDomainNode Node { get; }
        protected NodeTestFixture Fixture { get; }

        protected override void AfterAll()
        {
          //  Fixture.Output.WriteLine($"after test {GetType().Name}");
           // DumpMemoryUsage(Fixture.Output);
            Fixture.Dispose();
        }

       //private void DumpMemoryUsage(ITestOutputHelper output)
       //{
       //    ComputerInfo CI = new ComputerInfo();
       //    ulong mem = ulong.Parse(CI.AvailablePhysicalMemory.ToString());
       //
       //    var p = System.Diagnostics.Process.GetCurrentProcess();
       //    double f = 1024.0 * 1024;
       //    output.WriteLine($"Private memory size64: {p.PrivateMemorySize64 / f:#,##0}");
       //    output.WriteLine($"Working Set size64: {p.WorkingSet64 / f:#,##0}");
       //    output.WriteLine($"Available memory: {mem / f} MB");
       //    output.WriteLine($"Process: {p.ProcessName}");
       //}
    }
}