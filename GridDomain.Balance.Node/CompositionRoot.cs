

using System;
using Akka.Actor;
using GridDomain.Balance;
using GridDomain.Balance.ReadModel;
using GridDomain.CQRS.ReadModel;
using GridDomain.Node.Configuration;
using Microsoft.Practices.Unity;

namespace GridDomain.Balance.Node
{
    public static class CompositionRoot
    {
        public static void Init(IUnityContainer container,
                                IDbConfiguration conf)
        {
            //register all message handlers available to communicate
            //need to do it on plugin approach
            container.RegisterType<BalanceCommandsHandler>();

            Func<BusinessBalanceContext> contextFactory = () => new BusinessBalanceContext(conf.ReadModelConnectionString);

            container.RegisterType<IReadModelCreator<BusinessBalance>>(
                                    new InjectionFactory(c =>
                                        new ReadModelCreatorRetryDecorator<BusinessBalance>(
                                            new SqlReadModelCreator<BusinessBalance>(contextFactory))));

            container.RegisterType<BusinessCurrentBalanceProjectionBuilder>();
        }
    }
}