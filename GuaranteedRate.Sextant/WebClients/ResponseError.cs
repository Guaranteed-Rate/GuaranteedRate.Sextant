using System;
using System.Net.Http;


namespace GuaranteedRate.Sextant.WebClients
{
    //added just for convenience, if server returns an error we read it here and throw exception
    //with meaningfull response content
    class ResponseError
    {
        private HttpResponseMessage _response;

        public ResponseError(HttpResponseMessage response)
        {
            _response = response;
        }

        public void Check()
        {
            var responseBody = _response.Content.ReadAsStringAsync().Result;

            try
            {
                _response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Request failed. Response body: { responseBody }", ex);
            }
        }
    }
}
