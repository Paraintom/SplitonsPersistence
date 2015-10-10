﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using SplitonsPersistence.Events;

namespace SplitonsPersistence.Interface
{
    public class JsonLayer : IRequestManager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private IWebsocket Connection;
        public JsonLayer(IWebsocket connection)
        {
            this.Connection = connection;
            this.Connection.OnMessage += OnConnectionMessage;
        }

        public event EventHandler<RequestMessage> OnRequest;
        private void OnConnectionMessage(object sender, string s)
        {
            try
            {
                var request = JsonConvert.DeserializeObject<RequestMessage>(s);
                //dynamic request = JObject.Parse(s);
                if (!string.IsNullOrEmpty(request.request.projectId))
                {
                    logger.Info("Request {1} received for project {0}".FormatWith(request.request.projectId, request.id));

                    this.OnRequest.RaiseEvent(this, request);
                }
                else
                {
                    //error case : projectId is null or empty
                }
            }
            catch (Exception ex)
            {
                logger.Warn(string.Format("Bad incoming websocket message received : {0}", s), ex);
            }
        }

        public void Send(AnwserMessage answer)
        {
            string msg = JsonConvert.SerializeObject(answer);

            logger.Info("Sending answer for request {0} : {1}".FormatWith(answer.id, msg));
            Connection.Send(msg);
        }
    }
}
