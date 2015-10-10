using System;

namespace SplitonsPersistence.Interface
{
    public interface IRequestManager
    {
        event EventHandler<RequestMessage> OnRequest;
        void Send(AnwserMessage answer);
    }
}
