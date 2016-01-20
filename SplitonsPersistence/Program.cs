using System.Linq;
using System.Threading;
using NLog;
using SplitonsPersistence.Interface;
using SplitonsPersistence.Persistence;

namespace SplitonsPersistence
{
    class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static IProjectManager projectManager;
        static void Main(string[] args)
        {
            logger.Info("Starting application");
            projectManager = new ProjectManager(new FilePersister());
            IWebsocket connection = new RequestFlickerService("SplitonSync"); 
            IRequestManager requestManager = new JsonLayer(connection);
            requestManager.OnRequest += ProducerOnOnRequest;
            while (true)
            {
                Thread.Sleep(300000);
                NLog.LogManager.GetCurrentClassLogger().Info("I am still alive!(Providing some infos here...)");
            }
            
        }

        private static void ProducerOnOnRequest(object sender, RequestMessage request)
        {
            var websocket = (IRequestManager) sender;
            var toSend = projectManager.Update(request.request.projectId,
                request.request.lastUpdated, request.request.toUpdate);
            var splitonSynchRequest = new SplitonSynchRequest();
            //If there is no update, we do not change the lastUpdate
            var lastUpdated = request.request.lastUpdated;
            if (toSend.Count != 0)
            {
                //But if there is some change, the last update is the date of the last received update!
                lastUpdated = toSend.Select(o => o.lastUpdated).Max();
            }
            splitonSynchRequest.lastUpdated = lastUpdated;
            splitonSynchRequest.projectId = request.request.projectId;
            splitonSynchRequest.toUpdate = toSend;
            var answer = new AnwserMessage();
            answer.id = request.id;
            answer.answer = splitonSynchRequest;
            websocket.Send(answer);
        }
    }
}
