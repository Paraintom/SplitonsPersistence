using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            var result = toTest.Update(ProjectId, DateTime.MinValue.JavascriptTicks(), new List<Transaction>() { GetFakeTransaction(1) });
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
            var result = toTest.Update(ProjectId, DateTime.MinValue.JavascriptTicks(), new List<Transaction>() { GetFakeTransaction(1) });
            var savedOne = result.First();
            //We inject a old one, should be skiped
            Transaction outOfDateTransaction = GetFakeTransaction(1);
            outOfDateTransaction.lastUpdated = DateTime.Now.AddDays(-1).JavascriptTicks();
            result = toTest.Update(ProjectId, DateTime.Now.AddDays(-2).JavascriptTicks(), new List<Transaction>() { outOfDateTransaction });
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(savedOne, result.First());
        }
        [Test]
        public void NewTransactionReplace()
        {
            IProjectManager toTest = new ProjectManager(new InMemPersister());
            var result = toTest.Update(ProjectId, DateTime.MinValue.JavascriptTicks(), new List<Transaction>() { GetFakeTransaction(1) });
            var savedOne = result.First();
            Thread.Sleep(1);
            //We inject a old one, should be skiped
            Transaction newTransaction = GetFakeTransaction(1);
            newTransaction.amount++;
            result = toTest.Update(ProjectId, DateTime.Now.AddDays(-2).JavascriptTicks(), new List<Transaction>() { newTransaction });
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(((Transaction)savedOne).amount+1, ((Transaction)result.First()).amount);
        }

        public static int solution(int[] A)
        {
            var result = -1;
            for (int i = 0; i < A.Length; i++)
            {
                //Small performance improvment
                int maxPossibleDistance = A.Length - i;
                if (maxPossibleDistance <= result)
                {
                    //If we known we can't beat the current distance, no point to calculate it.
                    continue;
                }

                var currentValue = A[i];
                var currentCandidateIndice = -1;
                var currentTopValue = currentValue;
                var currentDownValue = currentValue;
                for (int j = i+1; j < A.Length; j++)
                {
                    int candidateValue = A[j];
                    if (candidateValue > currentValue)
                    {
                        //if not set || better that what we found before
                        if (currentTopValue == currentValue || currentTopValue >= candidateValue)
                        {
                            currentTopValue = candidateValue;
                            currentCandidateIndice = j;
                        }
                    }
                    if (candidateValue < currentValue)
                    {
                        //if not set || better that what we found before
                        if (currentDownValue == currentValue || currentDownValue <= candidateValue)
                        {
                            currentTopValue = candidateValue;
                            currentCandidateIndice = j;
                        }
                    }
                }
                var distance = currentCandidateIndice - i;
                if (distance > result)
                {
                    result = distance;
                }
            }
            return result;
        }


        [Test]
        public void TestA()
        {
            int[] i = new[] { 1, 4, 7, 3, 3, 5, 3, 5, 6, 3, 3, 5, 5, 5 };
            var result = solution(i);
            Assert.AreEqual(11, result);
        }

        private int solution(int A, int B)
        {
            if (A < 0)
            {
                A = 0;
            }
            var result = 0;
            var sq = Math.Ceiling(Math.Sqrt(A));
            while (Math.Pow(sq,2) < B)
            {
                result++;
                sq++;
            }
            return result;
        }

        [Test]
        public void TestAw()
        {
            var result1 = solution(-1, 10);
            var result = solution(0, 10);
            var result2 = solution(4, 17);
            Assert.AreEqual(result1, result);
            Assert.AreEqual(4, result);
            Assert.AreEqual(3, result2);
        }

        [Test]
        public void OnlyKeekTransactionOlderThanFrom()
        {
            IProjectManager toTest = new ProjectManager(new InMemPersister());
            var result = toTest.Update(ProjectId, DateTime.MinValue.JavascriptTicks(), new List<Transaction>()
            {
                GetFakeTransaction(1), GetFakeTransaction(2), GetFakeTransaction(3)
            });
            Assert.AreEqual(3, result.Count);
            //If we ask for them again we should see them :
            result = toTest.Update(ProjectId, DateTime.MinValue.JavascriptTicks(), new List<Transaction>());
            Assert.AreEqual(3, result.Count);
            //But if we just check the update, we should see nothing :
            Thread.Sleep(2);
            result = toTest.Update(ProjectId, DateTime.Now.JavascriptTicks(), new List<Transaction>());
            Assert.AreEqual(0, result.Count);
        }
    }
}
