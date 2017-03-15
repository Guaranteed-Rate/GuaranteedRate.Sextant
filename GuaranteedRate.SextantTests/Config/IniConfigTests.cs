using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuaranteedRate.Sextant.Config;
using NUnit.Framework;

namespace GuaranteedRate.SextantTests.Config
{
    [TestFixture]
    public class IniConfigTests
    {
        private IniConfig _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new IniConfig("testfile.txt");    
        }

        #region GetValue

        [Test]
        public void GivenLowerCaseValue_WhenGetValue_ThenExpectedValueReturned()
        {
            var expected = "myval";
            var configVal = "TestKey=" + expected;

            _sut.Init(configVal);

            var actual = _sut.GetValue("testkey");

            Assert.AreEqual(expected.ToLower(), actual);
        }

        [Test]
        public void GivenKeyThatDoesntExist_WhenGetValue_ThenNullReturned()
        {
            var actual = _sut.GetValue("testkey");

            Assert.AreEqual(null, actual);
        }

        [Test]
        public void GivenKeyThatDoesntExistWithDefault_WhenGetValue_ThenDefaultReturned()
        {
            var expected = "default";
            var actual = _sut.GetValue("testkey", expected);

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Init

        [Test]
        public void GivenLineWithoutSpaces_WhenInit_ThenExpectedValueSet()
        {
            var expected = "myval";
            var configVal = "TestKey=" + expected;

            _sut.Init(configVal);

            var actual = _sut.GetValue("TestKey");

            Assert.AreEqual(expected.ToLower(), actual);
        }

        [Test]
        public void GivenLine_WhenInit_ThenExpectedValueSet()
        {
            var expected = "myval";
            var configVal = "TestKey = " + expected;

            _sut.Init(configVal);

            var actual = _sut.GetValue("TestKey");

            Assert.AreEqual(expected.ToLower(), actual);
        }

        [Test]
        public void GivenLineWith2Equals_WhenInit_ThenExpectedValueSet()
        {
            var expected = "myval=anothervalue";
            var configVal = "TestKey = " + expected;

            _sut.Init(configVal);

            var actual = _sut.GetValue("TestKey");

            Assert.AreEqual(expected.ToLower(), actual);
        }

        #endregion
    }
}
