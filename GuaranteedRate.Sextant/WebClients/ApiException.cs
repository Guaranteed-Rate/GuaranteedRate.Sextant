using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace GuaranteedRate.Sextant.WebClients
{
    public class ApiException : Exception
    {
        public HttpResponseMessage Response { get; set; }
        public ApiException(HttpResponseMessage response)
        {
            Response = response;
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                return Response.StatusCode;
            }
        }

        public IEnumerable<string> Errors
        {
            get
            {
                return Data.Values.Cast<string>().ToList();
            }
        }

        public override string Message
        {
            get
            {
                return
                    $"StatusCode: {Response.StatusCode} ReasonPhrase: {Response.ReasonPhrase} Request: {Response.RequestMessage}";
            }
        }

        public static ApiException Create(HttpResponseMessage response)
        {
            var ex = new ApiException(response);

            try
            {
                var httpErrorObject = response.Content.ReadAsStringAsync().Result;

                // Create an anonymous object to use as the template for deserialization:
                var anonymousErrorObject =
                    new { message = "", ModelState = new Dictionary<string, string[]>() };

                // Deserialize:
                var deserializedErrorObject =
                    JsonConvert.DeserializeAnonymousType(httpErrorObject, anonymousErrorObject);


                // Sometimes, there may be Model Errors:
                if (deserializedErrorObject.ModelState != null)
                {
                    var errors =
                        deserializedErrorObject.ModelState
                                                .Select(kvp => string.Join(". ", kvp.Value));
                    for (int i = 0; i < errors.Count(); i++)
                    {
                        // Wrap the errors up into the base Exception.Data Dictionary:
                        ex.Data.Add(i, errors.ElementAt(i));
                    }
                }
                // Othertimes, there may not be Model Errors:
                else
                {
                    var error =
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(httpErrorObject);
                    foreach (var kvp in error)
                    {
                        // Wrap the errors up into the base Exception.Data Dictionary:
                        ex.Data.Add(kvp.Key, kvp.Value);
                    }
                }
            }
            catch (Exception)
            {
                //swallow json parsing exception.  The response has nothing else to offer us
            }

            return ex;
        }
    }
}
