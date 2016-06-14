using System;
using GridDomain.Node;
using GridDomain.Node.Configuration;

namespace GridDomain.Tests.Acceptance.Temp
{
    public class SchedulingDemoSpec : NodeCommandsTest
    {
        public SchedulingDemoSpec(string config, string name = null) : base(config, name)
        {
        }

        protected override TimeSpan Timeout { get; }

        protected override GridDomainNode GreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {
            throw new NotImplementedException();
        }
    }
}
