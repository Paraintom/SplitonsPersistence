using System;
using System.IO;
using System.Net;
using NLog;
using SplitonsPersistence.Configuration;
using SplitonsPersistence.Events;
using WebSocket4Net;
using ErrorEventArgs = SuperSocket.ClientEngine.ErrorEventArgs;
using Timer = System.Timers.Timer;

namespace SplitonsPersistence.Interface
{
    class RequestFlickerConnection : IRequestFlickerConnection
    {
        private string ServiceName;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private DateTime lastReceivedPong;
        private readonly WebSocket websocket;
        private const int checkTimeMs = 20000;
        private Timer t = new Timer(checkTimeMs);
        private bool pingOk;

        public event EventHandler<string> OnMessage;

        public RequestFlickerConnection(string serviceName)
        {
            pingOk = true;
            ServiceName = serviceName;
            string url = GetRequestFlickerUrl();
            websocket = new WebSocket(url);
            websocket.Opened += websocket_Opened;
            websocket.Error += websocket_Error;
            websocket.Closed += websocket_Closed;
            websocket.MessageReceived += websocket_MessageReceived;

            t.Elapsed += (sender, args) =>
            {
                try
                {
                    double totalMillisecondsSinceLastPing = (DateTime.Now - lastReceivedPong).TotalMilliseconds;
                    logger.Debug(string.Format("Checking pong received : {0}", totalMillisecondsSinceLastPing));
                    pingOk = totalMillisecondsSinceLastPing < 2 * checkTimeMs;
                    if (!pingOk)
                    {
                        logger.Debug(string.Format("Pong not Ok disconnecting."));
                        this.websocket.Close();
                        this.OnDisconnected.RaiseEvent(this,EventArgs.Empty);
                    }
                    else
                    {
                        websocket.Send("ping");
                    }
                }
                catch (Exception e)
                {
                    logger.Info("Unable to send a ping : " + e.Message);
                }
            };
            t.Start();
        }

        private void websocket_Opened(object sender, EventArgs e)
        {
            logger.Info(String.Format("The socket has been opened with ServiceName {0}.", this.ServiceName));
            websocket.Send(@"\\" + ServiceName);
            numberMessageReceived = 0;
            websocket.Send("ping");
        }
        private void websocket_Closed(object sender, EventArgs e)
        {
            logger.Warn("The socket has been closed for serviceName {0}, current state {1}");
            this.OnDisconnected.RaiseEvent(this,EventArgs.Empty);
        }

        private void websocket_Error(object sender, ErrorEventArgs e)
        {
            logger.Error(String.Format("Error with websocket : {0}", e.Exception.Message));
        }
        private int numberMessageReceived;
        private void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string message = e.Message;
            if (e.Message == "pong")
            {
                lastReceivedPong = DateTime.Now;
                return;
            }
            if (logger.IsDebugEnabled)
                logger.Debug(String.Format("Message received : {0}", message));
            //The first message is an echo of the subject, we can safely ignore it...
            if (numberMessageReceived != 0)
            {
                OnMessage.RaiseEvent(this, message);
            }
            numberMessageReceived++;
        }

        public void Send(string message)
        {
            try
            {
                if (websocket.State == WebSocketState.Open)
                {
                    websocket.Send(message);
                    //messageSent = true;
                }
            }
            catch (Exception e)
            {
                logger.Error(String.Format("Error when sending message : {0}", e.Message));
            }
        }

        public bool IsUp { get { return websocket.State == WebSocketState.Open && pingOk; } }
        public event EventHandler OnDisconnected;
        public void Start()
        {
            websocket.Open();
        }

        private static string GetRequestFlickerUrl()
        {
            string url;
            string configuredUrl = MyConfiguration.GetString(ConfigurationKeys.RequestFlickerUrl.ToString(), "ws://localhost:8181/");
            if (configuredUrl.StartsWith("http://"))
            {
                //Dynamic retrieval!
                string urlRequest = string.Format("{0}?get=RequestFlicker", configuredUrl);
                var result = WebRequest.Create(urlRequest).GetResponse().GetResponseStream();
                var stream = new StreamReader(result);
                String ContenuPageWeb = stream.ReadToEnd();
                logger.Info(string.Format("Response : {0}", ContenuPageWeb));
                url = String.Format("ws://{0}/", ContenuPageWeb.Trim());

                logger.Info(String.Format("Dynamic Url found: {0}", url));
            }
            else
            {
                if (configuredUrl.StartsWith("ws://"))
                {
                    url = configuredUrl;
                    logger.Info(String.Format("Url found in configuration {0}", url));
                }
                else
                {
                    throw new ApplicationException(string.Format("Invalid RequestFlicker Url : {0}", configuredUrl));
                }
            }
            return url;
        }

        public void Dispose()
        {
            try
            {
                t.Close();
                this.websocket.Close();
            }
            catch (Exception e) { }
        }
    }
}
