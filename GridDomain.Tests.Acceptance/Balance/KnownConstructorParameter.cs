using System.Reflection;
using Ploeh.AutoFixture.Kernel;

namespace GridDomain.Tests.Acceptance.Balance
{
    public class KnownConstructorParameter<TEntiry, TConstructorType> : ISpecimenBuilder
    {
        private readonly string _name;
        private readonly TConstructorType _value;

        public KnownConstructorParameter(string name, TConstructorType value)
        {
            _name = name;
            _value = value;
        }

        public object Create(object request, ISpecimenContext context)
        {
            var pi = request as ParameterInfo;
            if (pi == null)
                return new NoSpecimen();

            if (pi.Member.DeclaringType != typeof (TEntiry) ||
                pi.ParameterType != typeof (TConstructorType) ||
                pi.Name != _name)
                return new NoSpecimen();

            return _value;
        }
    }
}