using System;
using Akka.Actor;
using GridDomain.Domains;
using Microsoft.Extensions.Logging;

namespace GridDomain.Node
{
    public class NodeBuilder : INodeBuilder
    {
        public ILogger Logger { get; private set; }
        public IDomainConfiguration[] Configurations;
        public TimeSpan DefaultTimeout;
        public Func<ActorSystem> ActorProducers;

        public Action<ActorSystem> ActorInit = delegate { };
        private Action<ActorSystem> _transport = delegate {};

        public NodeBuilder()
        {
            DefaultTimeout = TimeSpan.FromSeconds(10);
            Configurations = new IDomainConfiguration[] { };
        }

        public INodeBuilder ActorSystem(Func<ActorSystem> sys)
        {
            ActorProducers = sys;
            return this;
        }
     
        public INodeBuilder Initialize(Action<ActorSystem> sys)
        {
            ActorInit += sys;
            return this;
        }

        //TODO:remove in favor of ActorSystemBuilder?
        public INodeBuilder Log(ILogger log)
        {
            Logger = log;
            return this;
        }

        public INodeBuilder Domains(params IDomainConfiguration[] domainConfigurations)
        {
            Configurations = domainConfigurations;
            return this;
        }
  
        public INodeBuilder Timeout(TimeSpan timeout)
        {
            DefaultTimeout = timeout;
            return this;
        }
    }
}