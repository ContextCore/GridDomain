namespace GridDomain.Node.Configuration.Akka.Hocon
{
    internal class EmptyConfig : IAkkaConfig
    {
        public string Build()
        {
            return "";
        }
    }
}