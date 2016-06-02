using System;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace BusinesNews.Tests.Acceptance
{
    public class BusinessNewsContainerTests : CompositionRootTests
    {
        protected override IUnityContainer CreateContainer(TransportMode mode, IDbConfiguration conf)
        {
            var container = new UnityContainer();
            GridDomain.Node.CompositionRoot.Init(container,
                                            ActorSystemBuilders[mode](),
                                            conf,
                                            mode);
            GridDomain.Balance.Node.CompositionRoot.Init(container, conf);
            return container;
        }
    }
}