using System;

namespace GridDomain.Tests.Acceptance.GridConsole
{
    public sealed class Isolated<T> : IDisposable
    {
        private AppDomain _domain;
        private T _value;

        public Isolated()
        {
            _domain = AppDomain.CreateDomain("Isolated:" + Guid.NewGuid(),
                                             null, AppDomain.CurrentDomain.SetupInformation);

            Type type = typeof(T);

            _domain.CreateInstance(type.Assembly.FullName, type.FullName);
        }

        public void Dispose()
        {
            if(_domain != null)
            {
                AppDomain.Unload(_domain);

                _domain = null;
            }
        }
    }
}