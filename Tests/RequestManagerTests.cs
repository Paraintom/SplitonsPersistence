using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework;
using SplitonsPersistence;
using SplitonsPersistence.Interface;

namespace Tests
{
    [TestFixture]
    public class RequestManagerTests : BaseTest
    {
        [Test]
        public void BadMessageIsIgnored()
        {
            int requestReceived = 0;
            string badMessage = "ha ha, not a Json request!";
            FakeWebsocket w = new FakeWebsocket();
            IRequestManager totest = new JsonLayer(w);
            totest.OnRequest += (sender, message) => requestReceived++;
            //Test when we receive shitty data
            w.SimulateReceived(badMessage);

            Assert.AreEqual(0, requestReceived);
        }

        [Test]
        public void GoodRequestIsForwarded()
        {
            var requestReceived = new RequestMessage();

            FakeWebsocket w = new FakeWebsocket();
            IRequestManager totest = new JsonLayer(w);
            totest.OnRequest += (sender, message) => requestReceived = message;
            //Test when we receive good data
            var synchRequest = new SplitonSynchRequest();
            synchRequest.lastUpdated = DateTime.Parse("2000-02-02").JavascriptTicks();
            synchRequest.projectId = "projectwq";
            synchRequest.toUpdate = new List<Transaction>() { GetFakeTransaction(1) };
            var request = new RequestMessage();
            request.id = 12;
            request.request = synchRequest;
            string serializeRequest = JsonConvert.SerializeObject(request);
            Write("Receiving "+ serializeRequest);
            w.SimulateReceived(serializeRequest);

            Assert.AreEqual(request.id, requestReceived.id);
            Assert.AreEqual(request.request.lastUpdated, requestReceived.request.lastUpdated);
            Assert.AreEqual(request.request.projectId, requestReceived.request.projectId);
            Assert.AreEqual(request.request.toUpdate, requestReceived.request.toUpdate);
        }
    }
}
