using System;

namespace GridDomain.Scheduling.WebUI
{
    public interface IWebUiWrapper : IDisposable
    {
        IDisposable Start();
    }
}