namespace GridDomain.Node.Configuration.Hocon
{
    internal class EmptyConfig : IHoconConfig
    {
        public string Build()
        {
            return "";
        }
    }
}