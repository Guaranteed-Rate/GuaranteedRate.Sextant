using System.IO;
using GuaranteedRate.Sextant.Config;
using NUnit.Framework;

namespace GuaranteedRate.Sextant.Tests.Configs
{
    [TestFixture]
    public class IniConfigTests
    {
        private IniConfig _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new IniConfig("Configs//TestIni.ini");
            _sut.Init(File.ReadAllText("Configs//TestIni.ini"));
        }

        [Test]
        public void GivenLowerCaseValue_WhenGetValue_ThenExpectedValueReturned()
        {
            var expected = "myval";

            var actual = _sut.GetValue("testkey");

            Assert.AreEqual(expected.ToLower(), actual);
        }

        [Test]
        public void GivenKeyThatDoesntExist_WhenGetValue_ThenNullReturned()
        {
            var actual = _sut.GetValue("testkey123");

            Assert.AreEqual(null, actual);
        }

        [Test]
        public void GivenKeyThatDoesntExistWithDefault_WhenGetValue_ThenDefaultReturned()
        {
            var expected = "default";
            var actual = _sut.GetValue("testkey1234", expected);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GivenLineWithoutSpaces_WhenInit_ThenExpectedValueSet()
        {
            var expected = "myval";

            var actual = _sut.GetValue("TestKey");

            Assert.AreEqual(expected.ToLower(), actual);
        }

        [Test]
        public void GivenLine_WhenInit_ThenExpectedValueSet()
        {
            var expected = "myval";

            var actual = _sut.GetValue("TestKey");

            Assert.AreEqual(expected.ToLower(), actual);
        }

        [Test]
        public void GivenLine_GoodOrgID_WhenInit_ThenExpectedValueSet()
        {
            var expected = "myval2";

            var actual = _sut.GetValue("TestKey", "wrongval", "org1");

            Assert.AreEqual(expected.ToLower(), actual);
        }

        [Test]
        public void GivenLine_GoodDeeperOrgID_WhenInit_ThenExpectedValueSet()
        {
            var expected = "myval3";

            var actual = _sut.GetValue("deeper.TestKey", "wrongval", "org1");

            Assert.AreEqual(expected.ToLower(), actual);
        }

        [Test]
        public void GivenLine_GoodOrgID_WhenInit_ThenDefaultValueSet()
        {
            var expected = "myval";

            var actual = _sut.GetValue("TestKey", "wrongval", "org2");

            Assert.AreEqual(expected.ToLower(), actual);
        }

        [Test]
        public void GivenLine_GoodDeeperOrgID_WhenInit_ThenDefaultValueSet()
        {
            var expected = "myval1";

            var actual = _sut.GetValue("deeper.TestKey", "wrongval", "org2");

            Assert.AreEqual(expected.ToLower(), actual);
        }

        [Test]
        public void GivenLineWith2Equals_WhenInit_ThenExpectedValueSet()
        {
            var expected = "myval=anothervalue";

            var actual = _sut.GetValue("TestKey2");

            Assert.AreEqual(expected.ToLower(), actual);
        }
    }
}
