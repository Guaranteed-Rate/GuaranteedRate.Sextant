using GuaranteedRate.Sextant.EncompassUtils;
using GuaranteedRate.Sextant.Exceptions;
using NUnit.Framework;

namespace GuaranteedRate.Sextant.Integration.Tests.EncompassUtils
{
    [TestFixture]
    [Explicit]
    public class SessionUtilsTests
    {
        [Test]
        public void GetEncompassSession_with_invalid_url_throws_connection_exception()
        {
            // arrange
            var encompassUrl = "https://TEBE11158xxx.ea.elliemae.net$TEBE11158xxx";
            var login = "testlo";
            var pw = "password";

            // act
            Assert.Throws<ServerConnectionException>(() =>
            {
                var actual = SessionUtils.GetEncompassSession(encompassUrl, login, pw);
            });
        }


        [Test]
        public void GetEncompassSession_with_invalid_login_throws_login_exception()
        {
            // arrange
            var encompassUrl = "https://TEBE11158661.ea.elliemae.net$TEBE11158661";
            var login = "testloooooo";
            var pw = "password";

            // act
            var ex = Assert.Throws<ServerLoginException>(() =>
            {
                var actual = SessionUtils.GetEncompassSession(encompassUrl, login, pw);
            });

            // assert error type
            Assert.AreEqual(ServerLoginException.ErrorTypes.UserNotFound, ex.ErrorType);
        }

        [Test]
        public void GetEncompassSession_with_invalid_password_throws_login_exception()
        {
            // arrange
            var encompassUrl = "https://TEBE11158661.ea.elliemae.net$TEBE11158661";
            var login = "testlo";
            var pw = "not_my_password";

            // act
            var ex = Assert.Throws<ServerLoginException>(() =>
            {
                var actual = SessionUtils.GetEncompassSession(encompassUrl, login, pw);
            });

            // assert error type
            Assert.AreEqual(ServerLoginException.ErrorTypes.InvalidPassword, ex.ErrorType);
        }

    }
}
