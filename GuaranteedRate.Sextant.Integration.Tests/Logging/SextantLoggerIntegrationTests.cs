using GuaranteedRate.Sextant.Config;
using GuaranteedRate.Sextant.Integration.Core;
using GuaranteedRate.Sextant.Logging;
using NUnit.Framework;

namespace GuaranteedRate.Sextant.Integration.Tests.Logging
{
    [TestFixture]
    [Explicit]
    class SextantLoggerIntegrationTests: EncompassSDKBaseTest
    {
        private IEncompassConfig _encompassConfig;
        private string _loggerName = "SextantLoggerIntegrationTests";

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _encompassConfig = new IntegrationEncompassConfig();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            Logger.Shutdown(30, true);
            //Serilog.Log.CloseAndFlush();
        }

        [Test]
        public void Constructor_with_config_only_returns_logger()
        {
            // act
            var actual = new SextantLogger(_encompassConfig, _loggerName);

            // assert
            Assert.NotNull(actual);
        }

        [Test]
        public void Warning_writes_to_loggly()
        {
            // arrange
            var logger = new SextantLogger(_encompassConfig, _loggerName);

            // act
            logger.Warning("Test warning from integration test");

            // assert
            Assert.Pass("Check loggly for results");
        }

        [Test]
        public void Info_writes_to_loggly()
        {
            // arrange
            var logger = new SextantLogger(_encompassConfig, _loggerName);

            // act
            logger.Info("Test info from integration test");

            // assert
            Assert.Pass("Check loggly for results");
        }

    }
}
