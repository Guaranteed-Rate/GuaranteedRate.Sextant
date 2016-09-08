using System;
using NUnit.Framework;

namespace GuaranteedRate.Sextant.Config.Tests
{
    [TestFixture()]
    public class JsonEncompassConfigTests
    {
        [Test()]
        public void ForValidConfigReturnGoodValues()
        {
            var json = "{\"foo\": \"bar\",\"boolkey\": \"false\",\"intkey\": \"4563\", \"Items\": [{\"FirstKey\": \"FirstVal\"}, {\"SecondKey\": \"SecondVal\"}] }";
            var config = new JsonEncompassConfig(json);
            Assert.That(config.GetKeys().Count ==8, String.Format("Expected 8, got {0}",config.GetKeys().Count));
            Assert.That(config.GetValue("foo").Equals("bar"), String.Format("Expected 3, got {0}", config.GetValue("foo")));
            Assert.That(config.GetValue("boolkey",true).Equals(false),String.Format("Expected 3, got {0}", config.GetValue("boolkey", true)));
            Assert.That(config.GetValue("Items.[0].FirstKey").Equals("FirstVal"));
        }
    }
}