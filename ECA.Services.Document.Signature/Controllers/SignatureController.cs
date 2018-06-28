using System;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Swashbuckle.AspNetCore.SwaggerGen;
using Newtonsoft.Json.Linq;
using ECA.Services.Document.Signature.Models;
using ECA.Services.Document.Signature.DocuSign;
using ECA.Services.Document.Signature.DocuSign.Exceptions;
using ECA.Services.Document.Signature.JsonHelpers;
using ECA.Services.Document.Signature.DocuSign.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using NLog;

namespace ECA.Services.Document.Signature.Controllers
{
    [Route("/")]
    public class SignatureController : Controller
    {
        private Logger _logger;
        public SignatureController()
        {
            _logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
        }

        [HttpGet("envelope/{envelopeId}/status")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(Response))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, typeof(Response))]
        public IActionResult GetEnvelopeStatus([FromServices]IStatusBuilder builder, string envelopeId)
        {
            try
            {
                string username, password;
                ExtractHeaderValues(Request.Headers, out username, out password);

                if (envelopeId == null)
                    throw new SigSvcParamError("Request format incorrect; check query string syntax passes envelopeId correctly.");

                IResponse response;
                if (builder.Build(envelopeId, username, password, out response))
                    return (ResultFormatter.Format(200, response));
                else
                    return (ResultFormatter.Format(400, response));
            }
            catch (SigSvcPostBodyError error)
            {
                _logger.Error(error, "GET envelope/{envelopeId}/status");
                return ResultFormatter.Format(400, error);
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "GET envelope/{envelopeId}/status");
                return ResultFormatter.Format(500, exc);
            }
        }

        [HttpGet("template/{templateId}")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(Response))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, typeof(Response))]
        public IActionResult GetTemplate([FromServices]ITemplateFieldBuilder builder, string templateId)
        {
            try
            {
                string username, password;
                ExtractHeaderValues(Request.Headers, out username, out password);

                if (templateId == null)
                    throw new SigSvcParamError("Request format incorrect; check query string syntax passes templateId correctly.");

                IResponse response;
                if (builder.Build(templateId, username, password, out response))
                    return (ResultFormatter.Format(200, response));
                else
                    return (ResultFormatter.Format(400, response));
            }
            catch ( SigSvcPostBodyError error)
            {
                _logger.Error(error, "GET template/{templateId}");
                return ResultFormatter.Format(400, error);
            }
            catch( Exception exc)
            {
                _logger.Error(exc, "GET template/{templateId}");
                return ResultFormatter.Format(500, exc);
            }
        }

        [HttpPost("signature")]          // send
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        public IActionResult PostSignature([FromServices]ISignatureBuilder builder, [FromBody]SignatureRequest body )         // aspnetcore built-in injection, see: startup.cs
        {
            try
            {
                string username, password;
                ExtractHeaderValues(Request.Headers, out username, out password);

                if (body == null)
                    throw new SigSvcPostBodyError("Error converting POST body json to SignatureRequest object.  Check structure of request, and Json in jsonlint.com for validity.");

                IResponse response;
                if (builder.Build(body, username, password, out response))
                    return (ResultFormatter.Format(200, response));
                else
                    return (ResultFormatter.Format(400, response));
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "POST signature");
                return ResultFormatter.Format(500, exc);
            }
        }

        [HttpGet("account")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(Response))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, typeof(Response))]
        public IActionResult GetAccount([FromServices]IRestApiWrapper api)
        {
            try
            {
                string username, password;
                ExtractHeaderValues(Request.Headers, out username, out password);

                string accountId = api.GetAccountId(username, password);

                Response response = new Response("accountId", accountId);
                response.Meta.Add("status", "200");
                return ResultFormatter.Format(200, response);
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "GET account");
                return ResultFormatter.Format(500, exc);
            }
        }

        [HttpGet("ping")]   // ping
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        public IActionResult GetPing()
        {
            _logger.Info("GET ping");
            return ResultFormatter.ResponseOK((new JProperty("Ping", "Success")));
        }
        [HttpGet("version")]   // assembly version
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        public IActionResult GetVersion()
        {
            _logger.Info("GET version");
            var assemblyVersion = typeof(Startup).Assembly.GetName().Version.ToString();
            return ResultFormatter.ResponseOK((new JProperty("Version", assemblyVersion)));
        }

        private void ExtractHeaderValues(IHeaderDictionary headers, out string username, out string password)
        {
            var docuSignCredentialsEncrypted = headers["DocuSignCredentials"];
            var docuSignCredentials = JObject.Parse(Security.Encryption.DecryptStringAes(docuSignCredentialsEncrypted));

            username = docuSignCredentials.Value<string>("DocuSignUsername");
            password = docuSignCredentials.Value<string>("DocuSignPassword");

            if (username == null || password == null)
                throw new SigSvcHeaderError("Request header error reading DocuSign Username or Password. Check header for DocuSignUsername and DocuSignPassword validity.");
        }
    }
}
