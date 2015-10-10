using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using SplitonsPersistence.Events;

namespace SplitonsPersistence.Interface
{
    class RequestFlickerService : IWebsocket
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private RequestFlickerConnection websocket;
        private readonly object internalLock = new object();

        public RequestFlickerService(string serviceName)
        {
            Logger.Debug("In constructor");
            ServiceName = serviceName;
            StartNewWebsocket();
            var success = TestConnectionSuccessFull();
            if (!success)
            {
                string errorString = string.Format("Cannot connect to RequestFlicker");
                Logger.Error(errorString);
                throw new ApplicationException(errorString);
            }
            //Reconnection logic
            Task.Factory.StartNew(Reconnect);
        }

        private void StartNewWebsocket()
        {
            if (websocket != null)
            {
                websocket.OnDisconnected -= OnDisconnected;
                websocket.OnMessage -= websocket_MessageReceived;
                websocket.Dispose();
            }
            websocket = new RequestFlickerConnection(ServiceName);
            websocket.OnDisconnected += OnDisconnected;
            websocket.OnMessage += websocket_MessageReceived;
            websocket.Start();
        }

        private void websocket_MessageReceived(object sender, string message)
        {
            if (Logger.IsDebugEnabled)
                Logger.Debug(String.Format("Message received : {0}", message));
            OnMessage.RaiseEvent(this,message);
        }

        private Task reconnectionTask;
        private void OnDisconnected(object sender, EventArgs e)
        {
            lock (internalLock)
            {
                Logger.Info(
                    "OnDisconnected for serviceName {0}, current state {1}.",
                    this.ServiceName, websocket.IsUp);
            }
        }

        private void Reconnect()
        {
            int tryNumber = 0;
            while (true)
            {
                try
                {
                    while (!websocket.IsUp)
                    {
                        tryNumber++;
                        TimeSpan waitPeriod = GetWaitPeriod(tryNumber);
                        Logger.Warn(string.Format("Connection failed, trying again in {0} s",
                            waitPeriod.TotalSeconds));
                        Thread.Sleep(waitPeriod);
                        if (!websocket.IsUp)
                        {
                            StartNewWebsocket();
                            Thread.Sleep(1000);
                        }
                    }
                    tryNumber = 0;

                }
                catch (Exception e)
                {
                    Logger.Error("Error on Reconnecting thread :", e);
                }
                Thread.Sleep(5000);
                if (!websocket.IsUp)
                {
                    Logger.Warn("We are Not Up! Trying to reconnect");
                }
            }
        }

        public string ServiceName
        {
            get;
            private set;
        }

        
        public event EventHandler<string> OnMessage;
        public void Send(string message)
        {
            websocket.Send(message);
        }

        private bool TestConnectionSuccessFull()
        {
            bool success = false;
            Logger.Warn("Trying to connect with ServiceName {0}, currentState {1}", this.ServiceName, websocket.IsUp);
            try
            {
                for (int i = 0; i < 5; i++)
                {
                    if (websocket.IsUp)
                    {
                        success = true;
                        break;
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("unable to connect to webSocket server : " + ex);
            }
            return success;
        }

        private static TimeSpan GetWaitPeriod(int tryNumber)
        {
            switch (tryNumber)
            {
                case 0:
                case 1:
                    return TimeSpan.FromSeconds(2);
                case 2:
                    return TimeSpan.FromSeconds(5);
                case 3:
                    return TimeSpan.FromSeconds(10);
                case 4:
                case 5:
                    return TimeSpan.FromSeconds(30);
                case 6:
                    return TimeSpan.FromSeconds(60);
                case 7:
                case 8:
                case 9:
                    return TimeSpan.FromSeconds(60 * 5);
                default:
                    return TimeSpan.FromSeconds(60 * 20);
            }
        }
    }
}
