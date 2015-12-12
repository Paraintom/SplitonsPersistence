using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework;
using SplitonsPersistence;
using SplitonsPersistence.Persistence;

namespace Tests
{
    [TestFixture]
    public class PersistenceTests : BaseTest
    {
        const string ProjectId = "projTest-002";

        [Test]
        public void ReadNonExistingProjectReturnsEmptyList()
        {
            IPersister toTest = new FilePersister();
            var result = toTest.Read("nothing!" + DateTime.Now.Ticks);
            Assert.IsEmpty(result);
        }

        [Test]
        public void TestPersistThenRead()
        {
            IPersister toTest = new FilePersister();
            var transactions = new List<UpdatableElement>();
            for (int i = 0; i < 10; i++)
            {
                transactions.Add(GetFakeTransaction(i));
            }
            toTest.Persist(ProjectId, transactions);
            var result = toTest.Read(ProjectId);
            Assert.AreEqual(result.Count, transactions.Count);
            var stringInput = JsonConvert.SerializeObject(transactions);
            var stringResult  = JsonConvert.SerializeObject(result);
            Assert.AreEqual(stringInput,stringResult);
        }

        [Test]
        public void TestSerial()
        {
            var result =
                JsonConvert.DeserializeObject<List<Transaction>>(
                    "[{\"id\":\"1\",\"lastUpdated\":1433428000717,\"from\":\"from1\",\"to\":[\"to1\"],\"comment\":\"Comment1\",\"amount\":1.0,\"currency\":\"ccy1\"}]");
            Assert.IsNotEmpty(result);
            var serial = JsonConvert.SerializeObject(result);
            Assert.IsNotNull(serial);
        }
    }
}
