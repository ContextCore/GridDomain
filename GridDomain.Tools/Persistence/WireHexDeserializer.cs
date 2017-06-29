using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wire;

namespace GridDomain.Tools.Persistence
{
    namespace Microsoft.EntityFrameworkCore
    {
        public interface IEntityTypeConfiguration<TEntity>where TEntity : class
        {
            void Map(EntityTypeBuilder<TEntity> builder);
        }

        public static class ModelBuilderExtensions
        {
            public static void AddConfiguration<TEntity>(this ModelBuilder modelBuilder, IEntityTypeConfiguration<TEntity> configuration)
                where TEntity : class
            {
                configuration.Map(modelBuilder.Entity<TEntity>());
            }
        }
    }
    internal class WireHexDeserializer
    {
        public T Deserialize<T>(string wireSerializedHexString)
        {
            var bytes = HexStringToByteArray(wireSerializedHexString);

            return Deserialize<T>(bytes);
        }

        public T Deserialize<T>(byte[] bytes)
        {
            var serializer = new Serializer();

            var obj = serializer.Deserialize<T>(new MemoryStream(bytes));
            return obj;
        }

        private byte[] HexStringToByteArray(string hex)
        {
            return
                Enumerable.Range(0, hex.Length)
                          .Where(x => x % 2 == 0)
                          .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                          .ToArray();
        }

        public static string ByteArrayToHexString(byte[] ba)
        {
            var hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }
    }
}