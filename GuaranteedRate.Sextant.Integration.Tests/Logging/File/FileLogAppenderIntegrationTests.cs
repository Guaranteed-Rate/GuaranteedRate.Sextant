using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GuaranteedRate.Sextant.Integration.Core;
using GuaranteedRate.Sextant.Logging;
using GuaranteedRate.Sextant.Logging.File;
using NUnit.Framework;

namespace GuaranteedRate.Sextant.Integration.Tests.Logging.File
{
    [TestFixture]
    public class FileLogAppenderIntegrationTests
    {
        [Test, Category("Integration")]
        public void WhenLog_ThenSuccess()
        {


            using (var sut = new FileLogAppender(new IntegrationEncompassConfig()))
            {
                Logger.AddAppender(sut);
                var po = new ParallelOptions();
                po.MaxDegreeOfParallelism = 2;
                Parallel.For(0, 200, po, a => LogIt());
                Thread.Sleep(30000);
                sut.Shutdown();
            }

        }

        private static Task<int> LogIt()
        {
            Console.WriteLine("Logging.....");
            var fields = new Dictionary<string, string>
                    {
                        {"Application", "SextantLogger"},
                        {
                            "Message",
                            "The original team behind the game Portal called the game Narbuncular Drop because the word Narbuncular is unique.  It would make a great test logging message too."
                        },
                        {"Company", "Guaranteed Rate"},
                        {"loggerName", "Guaranteed Rate Encompass Logger"}
                    };
            Logger.Info("IntegrationTests", $"test log entry from thread {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            return Task.FromResult(1);

        }
    }

}
