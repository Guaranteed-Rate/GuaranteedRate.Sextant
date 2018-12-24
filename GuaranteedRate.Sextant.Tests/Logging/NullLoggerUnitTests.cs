using GuaranteedRate.Sextant.Logging;
using NUnit.Framework;

namespace GuaranteedRate.Sextant.Tests.Logging
{
    [TestFixture]
    public class NullLoggerUnitTests
    {
        private ILogger _logger;
        private string _message = "Example message";

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _logger = new NullLogger();
        }

        [Test]
        public void Info_does_not_throw_exception()
        {
            // act
            Assert.DoesNotThrow(() => _logger.Info(_message));
        }

        [Test]
        public void Debug_does_not_throw_exception()
        {
            // act
            Assert.DoesNotThrow(() => _logger.Debug(_message));
        }

        [Test]
        public void Warning_does_not_throw_exception()
        {
            // act
            Assert.DoesNotThrow(() => _logger.Warning(_message));
        }

        [Test]
        public void Error_does_not_throw_exception()
        {
            // act
            Assert.DoesNotThrow(() => _logger.Error(_message));
        }

        [Test]
        public void Fatal_does_not_throw_exception()
        {
            // act
            Assert.DoesNotThrow(() => _logger.Fatal(_message));
        }

    }
}
