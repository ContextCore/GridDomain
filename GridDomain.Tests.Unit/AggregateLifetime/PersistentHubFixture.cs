using System;
using GridDomain.Configuration;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Configuration;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.AggregateLifetime
{
    public class PersistentHubFixture : NodeTestFixture
    {
        public PersistentHubFixture(ITestOutputHelper output, 
                                    IPersistentActorTestsInfrastructure infrastructure,
                                    TimeSpan? clearPeriod = null,
                                    TimeSpan? maxInactiveTime = null ):base(output)
        {
            Infrastructure = infrastructure;
            Add(CreateDomainConfiguration(clearPeriod ?? TimeSpan.FromSeconds(1), 
                                          maxInactiveTime ?? TimeSpan.FromSeconds(2)));
        }

        public IPersistentActorTestsInfrastructure Infrastructure { get; }

        
        private IDomainConfiguration CreateDomainConfiguration(TimeSpan childClearPeriod, TimeSpan childMaxInactiveTime)
        {
            var recycleCfg = new RecycleConfiguration(childClearPeriod, childMaxInactiveTime);
            var balloonDependencyFactory = new BalloonDependencies() {RecycleConfigurationCreator = 
                                                                              () => recycleCfg};
            
            var processManagerDependenciesFactory = new SoftwareProgrammingProcessDependenciesFactory();
            processManagerDependenciesFactory.StateDependencies.RecycleConfigurationCreator 
                = () => recycleCfg;

            return new DomainConfiguration(b => b.RegisterAggregate(balloonDependencyFactory),
                                           b => b.RegisterProcessManager(processManagerDependenciesFactory));
        }

    }
}