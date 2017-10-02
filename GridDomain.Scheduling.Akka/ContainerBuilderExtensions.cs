using Autofac;

namespace GridDomain.Scheduling.Akka {
    public static class ContainerBuilderExtensions
    {
        public static void RegisterType<TAbstract, TConcrete>(this ContainerBuilder builder)
        {
            builder.RegisterType<TConcrete>()
                   .As<TAbstract>();
        }
    }
}