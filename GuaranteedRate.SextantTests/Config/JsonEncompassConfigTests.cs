using System;
using NUnit.Framework;

namespace GuaranteedRate.Sextant.Config.Tests
{
    [TestFixture()]
    public class JsonEncompassConfigTests
    {
        private string _json = "        {       \"widget\": {    \"debug\": \"true\",    \"window\": {        \"title\": \"Sample Konfabulator Widget\",        \"name\": \"main_window\",        \"width\": 500,        \"height\": 500    },    \"image\": {         \"src\": \"Images/Sun.png\",        \"name\": \"sun1\",        \"hOffset\": 250,        \"vOffset\": 250,        \"alignment\": \"center\"    },    \"text\": {        \"data\": \"Click Here\",        \"size\": 36,        \"style\": \"bold\",        \"name\": \"text1\",        \"hOffset\": 250,        \"vOffset\": 100,        \"alignment\": \"center\",        \"onMouseUp\": \"sun1.opacity = (sun1.opacity / 100) * 90;\"    }}}    ";

        [Test()]
        public void ForValidConfigReturnGoodValues()
        {
            var config = new JsonEncompassConfig(_json);
            Assert.That(config.GetKeys().Count == 22, String.Format("Expected 22, got {0}", config.GetKeys().Count));
            Assert.That(config.GetValue("widget.debug", false).Equals(true),
                String.Format("Expected 'true' , got '{0}'", config.GetValue("widget.debug", false)));
            Assert.That(config.GetValue("widget.window.title").Equals("Sample Konfabulator Widget"));

            var subConfig = config.GetConfigGroup("widget.window");
            Assert.That(subConfig.GetValue("title").Equals("Sample Konfabulator Widget"));

        }



    }
}