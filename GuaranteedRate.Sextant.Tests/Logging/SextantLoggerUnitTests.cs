using GuaranteedRate.Sextant.Config;
using GuaranteedRate.Sextant.Integration.Core;
using GuaranteedRate.Sextant.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace GuaranteedRate.Sextant.Tests.Logging
{
    [TestFixture]
    public class SextantLoggerUnitTests
    {
        private IEncompassConfig _encompassConfig;
        private string _loggerName = "SextantUnitTests";

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _encompassConfig = new IntegrationEncompassConfig();
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
        public void Constructor_with_client_context_returns_logger()
        {
            // act
            var actual = new SextantLogger(_encompassConfig, _loggerName, "UnitTestClientID", "SextantLoggerUnitTests");

            // assert
            Assert.NotNull(actual);
        }

        [Test]
        public void Constructor_with_tags_returns_logger()
        {
            // arrange
            var additionalTags = new Dictionary<string, string>()
            {
                { "tag1", "value1" },
                { "tag2", "value2" }
            };

            // act
            var actual = new SextantLogger(_encompassConfig, _loggerName, additionalTags);

            // assert
            Assert.NotNull(actual);
        }


        [Test]
        public void Sample_Logger_Info_mock()
        {
            // arrange
            var loggerMock = new Mock<ILogger>();

            var textToBeLogged = "Some text to log here";
            var infoMessage = string.Empty;

            loggerMock.Setup(x => x.Info(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>())).Callback((string message) =>
            {
                infoMessage = message;
            }).Verifiable();

            // act
            loggerMock.Object.Info(textToBeLogged);

            // assert
            Assert.AreEqual(textToBeLogged, infoMessage);
        }

    }
}
