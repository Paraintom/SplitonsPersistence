using System;

namespace SplitonsPersistence.Interface
{
    public interface IWebsocket
    {
        event EventHandler<string> OnMessage;
        void Send(string message);
    }
}
