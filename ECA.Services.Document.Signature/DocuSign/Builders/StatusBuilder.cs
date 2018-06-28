using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ECA.Services.Document.Signature.Config;
using ECA.Services.Document.Signature.DocuSign.Exceptions;
using ECA.Services.Document.Signature.Models;
using Newtonsoft.Json.Linq;
using NLog;

namespace ECA.Services.Document.Signature.DocuSign.Builders
{
    public class StatusBuilder : IStatusBuilder
    {
        private IRestApiWrapper _api;
        private IJsonConfiguration _config;
        private Logger _logger;
        public StatusBuilder(IRestApiWrapper wrapper, IJsonConfiguration config)   // ctor
        {
            _api = wrapper;
            _config = config;
            _logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
        }
        public bool Build(string envelopeId, string username, string password, out IResponse response)
        {
            Response returnResponse = new Response();

            string accountId = _api.GetAccountId(username, password);
            string uri = $"{_config.ApiUrl}/v2/accounts/{accountId}/envelopes/{envelopeId}";

            JObject result;
            HttpStatusCode status = _api.SendDocuSignRequest(HttpMethod.Get, uri, username, password, out result);
            if (status != HttpStatusCode.OK)
            {
                string message = "Get Envelope status failed. Check envelopeId and Account header information and retry.";
                _logger.Error( $"Build() {message}");
                throw new SigSvcDocuSignAPICallFailure(message);
            }
            else
            {
                returnResponse.Data = new EnvelopeStatus(result["status"].Value<string>());
            }
            response = returnResponse;
            return true;
        }
    }
    public interface IStatusBuilder              // interface - really only used for injecting
    {
        bool Build(string envelopeId, string username, string password, out Models.IResponse response);
    }
}
