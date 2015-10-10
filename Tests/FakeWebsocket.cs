using System;
using SplitonsPersistence.Events;
using SplitonsPersistence.Interface;

namespace Tests
{
    public class FakeWebsocket : IWebsocket
    {
        public void SimulateReceived(string msg)
        {
            OnMessage.RaiseEvent(this,msg);
        }

        public event EventHandler<string> OnMessage;
        public event EventHandler<string> OnMessageSent;
        public void Send(string message)
        {
            OnMessageSent.RaiseEvent(this,message);
        }
    }
}
