
using Akka.Util.Internal;
using GridDomain.Scheduling.Akka;
using Microsoft.Practices.Unity;
using Quartz.Unity;

namespace GridDomain.Scheduling
{
    public class DependencyRegistrator
    {
        public static void Register(IUnityContainer container)
        {
            container.AddNewExtension<QuartzUnityExtension>();
            container.RegisterType<TargetActorProvider>().AsInstanceOf<ITargetActorProvider>();
        }
    }
}