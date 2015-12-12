using System;
using System.Diagnostics;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;
using NUnit.Framework;
using SplitonsPersistence;

namespace Tests
{
    public class BaseTest
    {
        public BaseTest()
        {
            ConsoleTarget target = new ConsoleTarget();
            target.Layout = "${time}|${level:uppercase=true}|${callsite:className=true:includeSourcePath=false:methodName=false}|${message}";

            SimpleConfigurator.ConfigureForTargetLogging(target, LogLevel.Trace);

            Logger logger = LogManager.GetLogger("ee");
            logger.Debug("another log message");

        }

        [SetUp]
        public virtual void Init()
        {
            //Init for every tests!
            Write("In Init");
        }

        public void Write(string message)
        {
            Debug.WriteLine(message);
        }

        public static UpdatableElement GetFakeTransaction(int number)
        {
            return GetFakeTransaction(number, number);
        }

        public static UpdatableElement GetFakeTransaction(int number, int amount)
        {
            var lastUpdated = DateTime.Now.JavascriptTicks();
            var transaction = new Transaction()
            {
                amount = amount,
                comment = "Comment" + number,
                currency = "ccy" + number, @from = "from" + number, to = new []{"to" + number}, id = number.ToString(),
                                       lastUpdated = lastUpdated
            };
            var jsonTransaction = JsonConvert.SerializeObject(transaction);
            return new UpdatableElement() { id = number.ToString(), lastUpdated = lastUpdated, SerializedValue = jsonTransaction };
        }
    }
}
