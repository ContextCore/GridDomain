namespace GridDomain.Node.Akka.Configuration.Hocon
{
    internal class EmptyConfig : IHoconConfig
    {
        public string Build()
        {
            return "";
        }
    }
}