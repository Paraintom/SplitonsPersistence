using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using NUnit.Framework;
using SplitonsPersistence;

namespace Tests
{
    [TestFixture]
    class ProjectManagerTests : BaseTest
    {
        const string ProjectId = "projTest-002";
        [Test]
        public void NewProjectWorks()
        {
            IProjectManager toTest = new ProjectManager(new InMemPersister());
            var result = toTest.Update(ProjectId, DateTime.MinValue.JavascriptTicks(), new List<UpdatableElement>() { GetFakeTransaction(1) });
            Assert.AreEqual(1, result.Count);
            Write(result.First().ToString());
        }

        [Test]
        public void TestGetTime()
        {
            DateTime now = DateTime.Now;
            var nowTick = now.JavascriptTicks();
            Write(nowTick.ToString());
            var dateParsed = nowTick.ToString().FromJavascriptTicks();
            Assert.IsTrue(Math.Abs((now - dateParsed).TotalSeconds) < 1);
        }

        [Test]
        public void OldTransactionSkipped()
        {
            IProjectManager toTest = new ProjectManager(new InMemPersister());
            var result = toTest.Update(ProjectId, DateTime.MinValue.JavascriptTicks(), new List<UpdatableElement>() { GetFakeTransaction(1) });
            var savedOne = result.First();
            //We inject a old one, should be skiped
            var outOfDateTransaction = GetFakeTransaction(1);
            outOfDateTransaction.lastUpdated = DateTime.Now.AddDays(-1).JavascriptTicks();
            result = toTest.Update(ProjectId, DateTime.Now.AddDays(-2).JavascriptTicks(), new List<UpdatableElement>() { outOfDateTransaction });
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(savedOne, result.First());
        }
        [Test]
        public void NewTransactionReplace()
        {
            IProjectManager toTest = new ProjectManager(new InMemPersister());
            var result = toTest.Update(ProjectId, DateTime.MinValue.JavascriptTicks(), new List<UpdatableElement>() { GetFakeTransaction(1) });
            var savedOne = result.First();
            Thread.Sleep(1);
            //We inject a new one, should override the previous one
            var newTransaction = GetFakeTransaction(1,2);
            result = toTest.Update(ProjectId, DateTime.Now.AddDays(-2).JavascriptTicks(), new List<UpdatableElement>() { newTransaction });
            Assert.AreEqual(1, result.Count);
            var firstTransaction = JsonConvert.DeserializeObject<Transaction>(savedOne.SerializedValue);
            var savedTransaction = JsonConvert.DeserializeObject<Transaction>(result.First().SerializedValue);
            Assert.AreEqual(firstTransaction.amount + 1, savedTransaction.amount);
        }

        [Test]
        public void OnlyKeekTransactionOlderThanFrom()
        {
            IProjectManager toTest = new ProjectManager(new InMemPersister());
            var result = toTest.Update(ProjectId, DateTime.MinValue.JavascriptTicks(), new List<UpdatableElement>()
            {
                GetFakeTransaction(1), GetFakeTransaction(2), GetFakeTransaction(3)
            });
            Assert.AreEqual(3, result.Count);
            //If we ask for them again we should see them :
            result = toTest.Update(ProjectId, DateTime.MinValue.JavascriptTicks(), new List<UpdatableElement>());
            Assert.AreEqual(3, result.Count);
            //But if we just check the update, we should see nothing :
            Thread.Sleep(2);
            result = toTest.Update(ProjectId, DateTime.Now.JavascriptTicks(), new List<UpdatableElement>());
            Assert.AreEqual(0, result.Count);
        }
    }
}
