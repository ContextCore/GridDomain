using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using Microsoft.Practices.Unity;
using Shop.Domain.DomainServices;
using Shop.Domain.Sagas;

namespace Shop.Composition
{
    public class ShopConfiguration : IContainerConfiguration
    {
        public void Register(IUnityContainer container)
        {
            var inMemoryPriceCalculator = new InMemoryPriceCalculator();
            container.RegisterInstance<IPriceCalculator>(inMemoryPriceCalculator);

            var buyNowSagaContainerConfiguration = SagaConfiguration.Instance<BuyNow, BuyNowData>(new BuyNowSagaFactory(inMemoryPriceCalculator),
                                                                                                  BuyNow.Descriptor,
                                                                                                  () => new EachMessageSnapshotsPersistencePolicy());
            container.Register(buyNowSagaContainerConfiguration);
        }
    }


    public class ShopRouteMap : IMessageRouteMap
    {
        public Task Register(IMessagesRouter router)
        {
            throw new System.NotImplementedException();
        }
    }
}