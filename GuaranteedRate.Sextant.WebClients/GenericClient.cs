using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GuaranteedRate.Sextant.WebClients
{
    public class GenericClient : IGenericClient
    {
        private HttpClient _client;
        public string BaseAddress { get; set; }

        public GenericClient()
        {

        }

        public GenericClient(string baseAddress)
        {
            Initialize(baseAddress);
        }

        public GenericClient(string baseAddress, string token)
        {
            Initialize(baseAddress, token);
        }

        public GenericClient(string baseAddress, AuthenticationHeaderValue authHeader = null)
        {
            Initialize(baseAddress, authHeader);
        }

        protected void Initialize(string baseAddress)
        {
            if (string.IsNullOrEmpty(baseAddress))
            {
                throw new ArgumentException("Must not be null or empty", baseAddress);
            }

            if (baseAddress.LastOrDefault() != '/')
            {
                baseAddress += "/";
            }

            BaseAddress = baseAddress;

            _client = new HttpClient
            {
                BaseAddress = new Uri(BaseAddress),
                Timeout = new TimeSpan(0, 10, 0)
            };

            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected void Initialize(string baseAddress, string token)
        {
            Initialize(baseAddress);

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(token);
            }
        }

        protected void Initialize(string baseAddress, AuthenticationHeaderValue authHeader)
        {
            Initialize(baseAddress);

            if (authHeader != null)
            {
                _client.DefaultRequestHeaders.Authorization = authHeader;
            }
        }

        public T Get<T>(string resource)
        {
            resource = StripLeadingSlash(resource);

            var response = _client.GetAsync(resource).Result;

            return UnpackModel<T>(response).Result;
        }

        public async Task<T> GetAsync<T>(string resource, CancellationToken? cancellationToken = null)
        {
            resource = StripLeadingSlash(resource);

            HttpResponseMessage response;

            if (cancellationToken.HasValue)
            {
                response = await _client.GetAsync(resource, cancellationToken.Value);
            }
            else
            {
                response = await _client.GetAsync(resource);
            }

            return await UnpackModel<T>(response);
        }

        public TOut Post<TOut>(string resource)
        {
            resource = StripLeadingSlash(resource);

            var response = _client.PostAsync(resource, null).Result;  // Blocking call!

            return UnpackModel<TOut>(response).Result;
        }

        public void Post<TIn>(string resource, TIn request)
        {
            resource = StripLeadingSlash(resource);

            var content = new ObjectContent<TIn>(request, new JsonMediaTypeFormatter());
            var response = _client.PostAsync(resource, content).Result;  // Blocking call!

            var model = UnpackModel<string>(response).Result; //needed to assess if an ApiException occurred
        }

        public TOut Post<TIn, TOut>(string resource, TIn request)
        {
            resource = StripLeadingSlash(resource);

            var content = new ObjectContent<TIn>(request, new JsonMediaTypeFormatter());
            var response = _client.PostAsync(resource, content).Result;  // Blocking call!

            return UnpackModel<TOut>(response).Result;
        }

        public Stream PostReturnStream<TIn>(string resource, TIn request)
        {
            resource = StripLeadingSlash(resource);

            var content = new ObjectContent<TIn>(request, new JsonMediaTypeFormatter());
            var response = _client.PostAsync(resource, content).Result;  // Blocking call!

            return UnpackStream(response).Result;
        }

        public async Task<TOut> PostAsync<TOut>(string resource, CancellationToken? cancellationToken = null)
        {
            resource = StripLeadingSlash(resource);
            HttpResponseMessage response;

            if (cancellationToken.HasValue)
            {
                response = await _client.PostAsync(resource, null, cancellationToken.Value);
            }
            else
            {
                response = await _client.PostAsync(resource, null);
            }

            return await UnpackModel<TOut>(response);
        }

        public async Task<TOut> PostAsync<TIn, TOut>(string resource, TIn request, CancellationToken? cancellationToken = null)
        {
            resource = StripLeadingSlash(resource);

            var content = new ObjectContent<TIn>(request, new JsonMediaTypeFormatter());
            HttpResponseMessage response;

            if (cancellationToken.HasValue)
            {
                response = await _client.PostAsync(resource, content, cancellationToken.Value);
            }
            else
            {
                response = await _client.PostAsync(resource, content);
            }

            return await UnpackModel<TOut>(response);
        }

        public TOut Put<TOut>(string resource)
        {
            resource = StripLeadingSlash(resource);

            var response = _client.PutAsync(resource, null).Result;  // Blocking call!

            return UnpackModel<TOut>(response).Result;
        }

        public TOut Put<TIn, TOut>(string resource, TIn request)
        {
            resource = StripLeadingSlash(resource);

            var content = new ObjectContent<TIn>(request, new JsonMediaTypeFormatter());
            var response = _client.PutAsync(resource, content).Result;  // Blocking call!

            return UnpackModel<TOut>(response).Result;
        }

        public async Task<TOut> PutAsync<TOut>(string resource, CancellationToken? cancellationToken = null)
        {
            resource = StripLeadingSlash(resource);
            HttpResponseMessage response;

            if (cancellationToken.HasValue)
            {
                response = await _client.PutAsync(resource, null, cancellationToken.Value);
            }
            else
            {
                response = await _client.PutAsync(resource, null);
            }

            return await UnpackModel<TOut>(response);
        }

        public async Task<TOut> PutAsync<TIn, TOut>(string resource, TIn request, CancellationToken? cancellationToken = null)
        {
            resource = StripLeadingSlash(resource);

            var content = new ObjectContent<TIn>(request, new JsonMediaTypeFormatter());
            HttpResponseMessage response;

            if (cancellationToken.HasValue)
            {
                response = await _client.PutAsync(resource, content, cancellationToken.Value);
            }
            else
            {
                response = await _client.PutAsync(resource, content);
            }

            return await UnpackModel<TOut>(response);
        }

        public T Delete<T>(string resource)
        {
            resource = StripLeadingSlash(resource);

            var response = _client.DeleteAsync(resource).Result; // Blocking call!

            return UnpackModel<T>(response).Result;
        }

        public async Task<T> DeleteAsync<T>(string resource, CancellationToken? cancellationToken = null)
        {
            resource = StripLeadingSlash(resource);

            HttpResponseMessage response;

            if (cancellationToken.HasValue)
            {
                response = await _client.DeleteAsync(resource, cancellationToken.Value);
            }
            else
            {
                response = await _client.DeleteAsync(resource);
            }

            return await UnpackModel<T>(response);
        }

        #region Helper Functions

        private string StripLeadingSlash(string resource)
        {
            if (resource.FirstOrDefault() == '/')
            {
                return resource.Remove(0, 1);
            }

            return resource;
        }

        private async Task<T> UnpackModel<T>(HttpResponseMessage message)
        {
            if (!message.IsSuccessStatusCode)
            {
                throw ApiException.Create(message);
            }

            if (message.StatusCode == HttpStatusCode.NotFound)
            {
                return default(T);
            }

            T model;

            var json = await message.Content.ReadAsStringAsync();
            var jsonSettings = new JsonSerializerSettings
            {
                Error = (sender, args) =>
                {
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                }
            };

            if (typeof(T) == typeof(string))
            {
                model = (T)Convert.ChangeType(json, typeof(T));
            }
            else if (!string.IsNullOrEmpty(json) && typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                var token = JToken.Parse(json);

                if (token.Type == JTokenType.Array)
                {
                    model = JsonConvert.DeserializeObject<T>(json, jsonSettings);
                }
                else
                {
                    try
                    {
                        model = JsonConvert.DeserializeObject<T>($"[{json}]", jsonSettings);
                    }
                    catch (Exception ex)
                    {
                        model = JsonConvert.DeserializeObject<T>(json, jsonSettings);
                    }
                    if (model == null && !string.IsNullOrEmpty(json))
                    {
                        throw new Exception($"Cannot deserialize model: {json} of type: {typeof(T)}");
                    }
                }
            }
            else
            {
                model = JsonConvert.DeserializeObject<T>(json, jsonSettings);
            }

            return model;
        }

        private async Task<Stream> UnpackStream(HttpResponseMessage message)
        {
            if (!message.IsSuccessStatusCode)
            {
                throw ApiException.Create(message);
            }

            if (message.StatusCode == HttpStatusCode.NotFound)
            {
                return default(Stream);
            }

            var stream = await message.Content.ReadAsStreamAsync();

            return stream;
        }

        #endregion
    }
}
