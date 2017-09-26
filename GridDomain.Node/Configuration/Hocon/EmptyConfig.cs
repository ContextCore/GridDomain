namespace GridDomain.Node.Configuration.Hocon
{
    internal class EmptyConfig : INodeConfig
    {
        public string Build()
        {
            return "";
        }
    }
}