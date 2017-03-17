using System;
using System.Collections.Generic;
using System.IO;
using GuaranteedRate.SextantTests.Config;
using NUnit.Framework;

namespace GuaranteedRate.Sextant.Config.Tests
{
    [TestFixture]
    public class JsonEncompassConfigTests
    {
        private JsonEncompassConfig _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new JsonEncompassConfig();
            _sut.Init(File.ReadAllText("Config\\TestJson.json"));
        }

        [Test]
        public void ForValidConfigReturnGoodValues()
        {
            Assert.That(_sut.GetKeys().Count == 28, $"Expected 28, got {_sut.GetKeys().Count}");
            Assert.That(_sut.GetValue("widget.debug", false).Equals(true), $"Expected 'true' , got '{_sut.GetValue("widget.debug", false)}'");
            Assert.That(_sut.GetValue("widget.window.title").Equals("Sample Konfabulator Widget"));

            var subConfig = _sut.GetConfigGroup("widget.window");
            Assert.That(subConfig.GetValue("title").Equals("Sample Konfabulator Widget"));
        }

        [Test]
        public void WhenGetValueT_ThenValueTReturned()
        {
            var model = _sut.GetValue<WindowModel>("widget.window");

            Assert.IsNotNull(model);
        }

        [Test]
        public void GivenInvalidPath_WhenGetValueT_ThenReturnDefault()
        {
            var model = _sut.GetValue<WindowModel>("badPath.window");

            Assert.IsNull(model);
        }

        [Test]
        public void GivenInvalidPath_WhenGetValueTList_ThenReturnDefault()
        {
            var model = _sut.GetValue<List<WindowModel>>("badPath.window");

            Assert.IsNull(model);
        }

        [Test]
        public void GivenInvalidPath_WhenGetValueTArray_ThenReturnDefault()
        {
            var model = _sut.GetValue<WindowModel[]>("badPath.window");

            Assert.IsNull(model);
        }

        [Test]
        public void WhenGetValueTList_ThenReturnList()
        {
            var model = _sut.GetValue<List<WindowModel>>("widget.windowList");

            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count);
        }

        [Test]
        public void WhenGetValueTArray_ThenReturnArray()
        {
            var model = _sut.GetValue<WindowModel[]>("widget.windowList");

            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Length);
        }


        [Test]
        public void WhenGetValueTIList_ThenReturnTIList()
        {
            var model = _sut.GetValue<IList<WindowModel>>("widget.windowList");

            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count);
        }

        [Test]
        public void GivenInvalidPath_WhenGetValueTIList_ThenReturnDefault()
        {
            var model = _sut.GetValue<IList<WindowModel>>("badPath.windowList");

            Assert.IsNull(model);
        }

        [Test]
        public void GivenInvalidPathWithDefaultReturnVal_WhenGetValueT_ThenReturnDefaultReturnVal()
        {
            var expected = new WindowModel {Height = 1, Name = "test", Title = "test title", Width = 1};
            var actual = _sut.GetValue<WindowModel>("widgets.window", expected);

            Assert.AreEqual(expected, actual);
        }
    }
}