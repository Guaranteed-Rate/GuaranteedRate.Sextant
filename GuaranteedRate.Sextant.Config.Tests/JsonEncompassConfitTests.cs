using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            _sut.Init(File.ReadAllText("Configs//TestJson.json"));
        }

        [Test]
        public void ForValidConfigReturnGoodValues()
        {
            Assert.That(_sut.GetKeys().Count == 43, $"Expected 43, got {_sut.GetKeys().Count}");
            Assert.That(_sut.GetValue("widget.debug", false).Equals(true), $"Expected 'true' , got '{_sut.GetValue("widget.debug", false)}'");

            Assert.That(_sut.GetValue<bool>("widget.debug", false).Equals(true), $"Expected 'true' , got '{_sut.GetValue("widget.debug", false)}'");
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
        public void GivenEmptyArray_WhenGetValueTArray_ThenReturnsEmptyArray()
        {
            var actual = _sut.GetValue<WindowModel[]>("widget.emptyWindowList");

            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Length);
        }

        [Test]
        public void GivenEmptyArray_WhenGetValueTList_ThenReturnsEmptyList()
        {
            var actual = _sut.GetValue<List<WindowModel>>("widget.emptyWindowList");

            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [Test]
        public void GivenEmptyArray_WhenGetValueTIList_ThenReturnsEmptyIList()
        {
            var actual = _sut.GetValue<IList<WindowModel>>("widget.emptyWindowList");

            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count);
        }

        [Test]
        public void GivenEmptyStringArray_WhenGetValueTArray_ThenReturnsEmptyArray()
        {
            var actual = _sut.GetValue<WindowModel[]>("widget.emptyStringWindowList");

            Assert.IsNull(actual);
        }

        [Test]
        public void GivenEmptyStringArray_WhenGetValueTList_ThenReturnsEmptyList()
        {
            var actual = _sut.GetValue<List<WindowModel>>("widget.emptyStringWindowList");

            Assert.IsNull(actual);
        }

        [Test]
        public void GivenEmptyStringArray_WhenGetValueTIList_ThenReturnsEmptyIList()
        {
            var actual = _sut.GetValue<IList<WindowModel>>("widget.emptyStringWindowList");

            Assert.IsNull(actual);
        }

        [Test]
        public void GivenNullArray_WhenGetValueTArray_ThenReturnsNull()
        {
            var actual = _sut.GetValue<WindowModel[]>("widget.nullWindowList");

            Assert.IsNull(actual);
        }

        [Test]
        public void GivenNullArray_WhenGetValueTList_ThenReturnsNull()
        {
            var actual = _sut.GetValue<List<WindowModel>>("widget.nullWindowList");

            Assert.IsNull(actual);
        }

        [Test]
        public void GivenNullArray_WhenGetValueTIList_ThenReturnsNull()
        {
            var actual = _sut.GetValue<IList<WindowModel>>("widget.nullWindowList");

            Assert.IsNull(actual);
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
            var expected = new WindowModel { Height = 1, Name = "test", Title = "test title", Width = 1 };
            var actual = _sut.GetValue<WindowModel>("widgets.window", expected);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GivenKeyWithHtmlVal_WhenGetValue_ThenHtmlValueReturned()
        {
            const string expected = "http://www.google.com";

            var actual = _sut.GetValue("HtmlTester.Url");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GivenBoolTrue_WhenGetValueBool_ThenReturnTrue()
        {
            var actual = _sut.GetValue<bool>("trueProp");

            Assert.IsTrue(actual);
        }

        [Test]
        public void GivenStringTrue_WhenGetValueBool_ThenReturnTrue()
        {
            var actual = _sut.GetValue<bool>("trueStringProp");

            Assert.IsTrue(actual);
        }

        [Test]
        public void GivenBoolFalse_WhenGetValueBool_ThenReturnFalse()
        {
            var actual = _sut.GetValue<bool>("falseProp");

            Assert.IsFalse(actual);
        }

        [Test]
        public void GivenStringFalse_WhenGetValueBool_ThenReturnFalse()
        {
            var actual = _sut.GetValue<bool>("falseStringProp");

            Assert.IsFalse(actual);
        }

        [Test]
        public void GivenInt_WhenGetValueInt_ThenReturnInt()
        {
            var actual = _sut.GetValue<int>("intProp");

            Assert.AreEqual(1, actual);
        }

        [Test]
        public void GivenStringInt_WhenGetValueInt_ThenReturnInt()
        {
            var actual = _sut.GetValue<int>("intStringProp");

            Assert.AreEqual(1, actual);
        }

        [Test]
        public void GivenNullInt_WhenGetValueInt_ThenReturnInt()
        {
            var actual = _sut.GetValue<int>("intNullProp");

            Assert.AreEqual(0, actual);
        }

        [Test]
        public void GivenNullInt_WhenGetValueNullableInt_ThenReturnNullInt()
        {
            var actual = _sut.GetValue<int?>("intNullProp");

            Assert.IsNull(actual);
        }

        [Test]
        public void GivenDouble_WhenGetValueInt_ThenReturnInt()
        {
            var actual = _sut.GetValue<double>("doubleProp");

            Assert.AreEqual(1.25, actual);
        }

        [Test]
        public void GivenStringDouble_WhenGetValueInt_ThenReturnInt()
        {
            var actual = _sut.GetValue<double>("doubleStringProp");

            Assert.AreEqual(1.25, actual);
        }

        [Test]
        public void GivenNullWindow_WhenGetValueWindow_ThenReturnNull()
        {
            var actual = _sut.GetValue<WindowModel>("nullWindow");

            Assert.IsNull(actual);
        }
    }
}
