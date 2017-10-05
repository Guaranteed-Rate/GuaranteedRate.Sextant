using System.Collections.Generic;
using GuaranteedRate.Sextant.Integration.Core;
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
                var fields = new Dictionary<string, string>
                {
                    {"Application", "SextantLogger"},
                    {"Message", "The original team behind the game Portal called the game Narbuncular Drop because the word Narbuncular is unique.  It would make a great test logging message too." },
                    {"Company", "Guaranteed Rate"},
                    {"loggerName", "Guaranteed Rate Encompass Logger"}
                };
                sut.Log(fields);
            }
        }
    }
}
