using System;
using GridDomain.Common;

namespace GridDomain.Aggregates
{
    public class AggregateAddress : IAggregateAddress
    {
        private readonly string _stringValue;

        public AggregateAddress(string typeName, string id)
        {
            Name = typeName;
            Id = id;
            _stringValue = Name + Separator + id;
        }

        public AggregateAddress(Type t, string id) : this(t.BeautyName(), id)
        {
            
        }

        public string Name { get; }
        public string Id { get; }

        private const char Separator = '_';
        public override string ToString() => _stringValue;

        public static AggregateAddress Parse(string fullName)
        {
            var parts = fullName.Split(Separator);
            if (parts.Length != 2)
                throw new BadAggregateAddressFormatException();
            var name = parts[0];
            var id = parts[1];
            return new AggregateAddress(name, id);
        }

        public static AggregateAddress New<T>(string id)
        {
            return new AggregateAddress(typeof(T), id);
        }
        public static AggregateAddress Parse<T>(string fullName)
        {
            var address = Parse(fullName);
            if(address.Name != typeof(T).BeautyName())
                throw new TypeMismatchException();
            return address;
        }

        public class TypeMismatchException : Exception
        {
        }
        public class BadAggregateAddressFormatException : Exception
        {
        }
    }


}