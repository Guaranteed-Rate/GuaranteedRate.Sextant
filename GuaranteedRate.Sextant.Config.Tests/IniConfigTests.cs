using System;
using System.IO;
using NUnit.Framework;

namespace GuaranteedRate.Sextant.Config.Tests
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
        public void GivenLineWith2Equals_WhenInit_ThenExpectedValueSet()
        {
            var expected = "myval=anothervalue";

            var actual = _sut.GetValue("TestKey2");

            Assert.AreEqual(expected.ToLower(), actual);
        }
    }
}
