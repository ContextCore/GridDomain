using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using GridDomain.Common;

namespace GridDomain.Aggregates
{
    public class AggregateAddress : IAggregateAddress
    {
        private readonly string _stringValue;

        public AggregateAddress(string name, string id)
        {
            this.Name = name;
            Id = id;
            _stringValue = this.Name + Separator + id;
        }

        public string Name { get; }
        public string Id { get; }

        private const string Separator = "_";
        private static readonly string[] SeparatorArray = new []{Separator};

        public override string ToString() => _stringValue;

        public static AggregateAddress New(Type type, string id)
        {
            return new AggregateAddress(type.BeautyName(), id);
        }
        
        //maybe introduce a regex? 
        public static AggregateAddress Parse(string fullName)
        {
            var parts = fullName.Split(SeparatorArray, StringSplitOptions.None);
            
            if (parts.Length < 2)
                throw new BadAggregateAddressFormatException();
            
            var name = parts.First();
            if(string.IsNullOrWhiteSpace(name))
                throw new BadAggregateAddressFormatException();
            
            var id = fullName.Substring(name.Length+1);
            if(string.IsNullOrWhiteSpace(id))
                throw new BadAggregateAddressFormatException();
            
            return new AggregateAddress(name, id);
        }
        
        public static AggregateAddress New<T>(string id)
        {
            return New(typeof(T), id);
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