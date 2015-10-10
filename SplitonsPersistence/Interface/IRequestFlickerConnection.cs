using System;

namespace SplitonsPersistence.Interface
{
    interface IRequestFlickerConnection : IWebsocket, IDisposable
    {
        bool IsUp { get; }
        event EventHandler OnDisconnected;
        void Start();
    }
}
