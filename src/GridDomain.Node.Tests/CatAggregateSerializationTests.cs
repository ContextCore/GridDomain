using GridDomain.Node.Akka;
using Newtonsoft.Json;
using Xunit;

namespace GridDomain.Node.Tests
{
    public class CatAggregateSerializationTests
    {
        [Fact]
        public void Commands_are_serializable()
        {
            var cmd = new Cat.GetNewCatCommand("mau");
            var jsonString = JsonConvert.SerializeObject(cmd);
            var restoredCmd = JsonConvert.DeserializeObject(jsonString);
        }
        
        [Fact]
        public void ShardedCommands_are_serializable()
        {
            var cmd = ShardedAggregateCommand.New(new Cat.GetNewCatCommand("mau"));
            var jsonString = JsonConvert.SerializeObject(cmd);
            var restoredCmd = JsonConvert.DeserializeObject(jsonString);
        }
    }
}