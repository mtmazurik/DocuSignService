using ECA.Services.Document.Signature.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using ECA.Services.Document.Signature.Config;
using Newtonsoft.Json.Linq;
using ECA.Services.Document.Signature.DocuSign.Exceptions;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using NLog;

namespace ECA.Services.Document.Signature.DocuSign
{
    public class RestApiWrapper : IRestApiWrapper
    {
        private HttpClient _client;
        private IJsonConfiguration _config;
        private Logger _logger;

        public RestApiWrapper(HttpClient client, IJsonConfiguration config)    //ctor
        {
            _client = client;
            _config = config;
            _logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
        }

        // Re-usable convenience routines.  Inject this IRestApiWrapper class, to reuse in Builder pattern classes
        public HttpStatusCode SendDocuSignRequest(HttpMethod method, string uri, string username, string password, out JObject retResponseData, JObject bodyData = null)
        {
            HttpResponseMessage result = null;
            HttpContent body = null;

            if (method == HttpMethod.Post || method == HttpMethod.Put)
            { 
                body = new StringContent(JsonConvert.SerializeObject(bodyData), Encoding.UTF8, "application/json");
            }

            result = _client.SendAsync(FormatRequest(method, uri, username, password, body)).Result;
            retResponseData = JObject.Parse(result.Content.ReadAsStringAsync().Result);

            return result.StatusCode;
        }

        public string GetAccountId(string username, string password )
        {
            string uri = $"{_config.ApiUrl}/v2/login_information?api_password=true";

            HttpStatusCode status = this.SendDocuSignRequest(HttpMethod.Get, uri, username, password, out JObject response);
            if (status != HttpStatusCode.OK)
                throw new SigSvcAccountAccessFailure("Account/Username access failure. Check username and password.");

            string accountId = response["loginAccounts"][0].Value<string>("accountId");
            _logger.Debug($"GetAccountId() Username:{username} AccountId:{accountId} successfully called.");
            return accountId ;
        }
        private HttpRequestMessage FormatRequest(HttpMethod method, string uri, string username, string password, HttpContent content = null)
        {
            HttpRequestMessage retRequest = new HttpRequestMessage()
            {
                RequestUri = new Uri(uri),
                Method = method
            };

            retRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            retRequest.Headers.Add(@"X-DocuSign-Authentication", $"{{ \"Username\": \"{username}\",\"Password\":\"{password}\",\"IntegratorKey\":\"{_config.IntegrationKey}\"}}");

            if (method == HttpMethod.Post || method == HttpMethod.Put)
            {
                retRequest.Content = content;
            }
            return retRequest;
        }
    }
}
