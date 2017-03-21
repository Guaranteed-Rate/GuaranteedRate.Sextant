using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Xml.Linq;
using EllieMae.EMLite.ClientServer;
using EllieMae.EMLite.RemotingServices;
using EllieMae.EMLite.Serialization;

namespace GuaranteedRate.Sextant.WebClients
{
    public class RuleProxy
    {
        private BizRuleInfo _bizRuleInfo;
        private string _resourceName;

        public RuleProxy(BizRuleInfo ruleInfo, string resourceName)
        {
            _resourceName = resourceName;
            _bizRuleInfo = ruleInfo;
        }

        public BizRuleInfo RuleInfo { get { return _bizRuleInfo; } }

        /// <summary>
        /// Loads the rule content from Encompass webservice.
        /// </summary>
        public XDocument Export()
        {
            var client = CreateHttpClient();

            var exportURL = $"/{_resourceName}/{_bizRuleInfo.RuleID}?format=json";

            //console application doesn't support async/await so blocking here.
            //once this app is integrated into a runner which supports synchronization context,
            //this should be reimplemented to 'await'.
            using (var result = client.GetAsync(exportURL).Result)
            {
                new ResponseError(result).Check();

                //console application doesn't support async/await so blocking here.
                //once this app is integrated into a runner which supports synchronization context,
                //this should be reimplemented to 'await'.
                var str = result.Content.ReadAsStringAsync().Result;

                using (var reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(str), new XmlDictionaryReaderQuotas()))
                {
                    var data = XElement.Load(reader).Value;

                    return XDocument.Parse(data);
                }
            }
        }

        public string SimpleExport()
        {
            var client = CreateHttpClient();

            var exportURL = $"/{_resourceName}/{_bizRuleInfo.RuleID}?format=xml";

            //console application doesn't support async/await so blocking here.
            //once this app is integrated into a runner which supports synchronization context,
            //this should be reimplemented to 'await'.
            using (var result = client.GetAsync(exportURL).Result)
            {
                new ResponseError(result).Check();

                //console application doesn't support async/await so blocking here.
                //once this app is integrated into a runner which supports synchronization context,
                //this should be reimplemented to 'await'.
                var str = result.Content.ReadAsStringAsync().Result;

                using (var reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(str), new XmlDictionaryReaderQuotas()))
                {
                    var data = XElement.Load(reader).Value;

                    return data;
                }
            }

        }

        /// <summary>
        /// Imports the rule into Encompass. The new rule would be created (with one exception if the rule with 
        /// exactly same name already exists in Encompass. In this case seems rule is being overwritten)
        /// Xml must be in the same format as the one returned by Export() method.
        /// </summary>
        public void Import(XDocument rule)
        {
            var client = CreateHttpClient();

            var importUrl = $"/{_resourceName}";

            var businessRuleObject = rule.ToString();

            using (var content = new StringContent(new JavaScriptSerializer().Serialize(businessRuleObject), Encoding.UTF8, "application/json"))
            {
                //console application doesn't support async/await so blocking here.
                //once this app is integrated into a runner which supports synchronization context,
                //this should be reimplemented to 'await'.
                using (var result = client.PostAsync(importUrl, content).Result)
                {
                    new ResponseError(result).Check();
                }
            }
        }

        private static HttpClient CreateHttpClient()
        {
            //this uri is the same for dev, stage and prod environments.
            var restApiBaseUri = "https://encompass-ep9.api.elliemae.com";
            var accessToken = $"{Session.StartupInfo.ServerInstanceName}_{Session.StartupInfo.SessionID}";

            var client = new HttpClient
            {
                BaseAddress = new Uri(restApiBaseUri)
            };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("json/application"));
            client.DefaultRequestHeaders.Add("elli-session", accessToken);

            return client;
        }

        //just found a way of getting another xml representation of rule.
        //that's not usable for importing rules to Encompass, but may come in handy
        //for debugging.
        public string GetSerializedRuleInfo()
        {
            var xmlSerialization = (XmlSerializationInfo)Activator.CreateInstance(typeof(XmlSerializationInfo), true);

            _bizRuleInfo.GetXmlObjectData(xmlSerialization);

            return xmlSerialization.ToString();
        }
    }
}