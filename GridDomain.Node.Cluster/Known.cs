using System;
using GridDomain.Common;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.ProcessManagers;

namespace GridDomain.Node.Cluster {
    public static class Known
    {
        public static class Paths
        {
            public static string ShardRegion(Type t)
            {
                return "system/sharding/" + t.BeautyName();
            }

            public static string ProcessShardRegion<TProcess,TState>() where TProcess : IProcess<TState> where TState : IProcessState
            {
                return ShardRegion(typeof(TProcess));
            }
            public static string AggregateShardRegion<TAggregate>() where TAggregate : IAggregate
            {
                return ShardRegion(typeof(TAggregate));
            }
        }

        public static class Names
        {
            public static string Region(Type t)
            {
                return t.BeautyName();
            }

            public static string EmptyProcess { get; } = "creator";
        }
        
    }
}