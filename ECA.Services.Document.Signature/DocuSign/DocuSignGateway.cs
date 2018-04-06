using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Model;
using ECA.Services.Document.Signature.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ECA.Services.Document.Signature.DocuSign
{
    public class DocuSignGateway : IDocuSignGateway
    {
        private const string SIGNER_ROLE = "Signer";                                // limitation:  Signer role is hard-coded
        private const string RECIPIENT_ID = "1";                                    // limitation:  one recipient is hard-coded
        private const string STATUS_SENT = "sent";                                            
        private String _integrationKey = "00000000-0000-0000-0000-000000000000";    // default: overriden by appsettings.json
        private String _apiUrl = "https://demo.docusign.net/restapi";               // default: overriden by appsettings.json
        private ApiClient _apiClient;
        private IEnvelopesApi _envelopesApi;
        private IAuthenticationApi _authApi;

        public DocuSignGateway( IAuthenticationApi authApi, IEnvelopesApi envelopesApi )   // ctor
        {
            _authApi = authApi;
            _envelopesApi = envelopesApi;
        }
        public Response Send( SignatureRequest request )
        {
            try
            {
                GetConfigSettings(out _apiUrl, out _integrationKey);
                _apiClient = new ApiClient(_apiUrl);
                EnvelopeSummary envelopeSummary = _envelopesApi.CreateEnvelope(Login(request.DocuSignUsername, request.DocuSignPassword, _integrationKey)
                                                                    , CreateEnvelopeDefinition(request));
                return FormatEnvelopeResponse(envelopeSummary);
            }
            catch(Exception exc)
            {
                return FormatExceptionResponse(exc);
            }
        }
        private string Login( string username, string password, string key)
        {
            SetAuthHeader(username, password, key);
            LoginInformation loginInfo = _authApi.Login();
            return loginInfo.LoginAccounts[0].AccountId;                // return first account ID (user may have several)       
        }
        private EnvelopeDefinition CreateEnvelopeDefinition(SignatureRequest body)
        {
            EnvelopeDefinition envelopeDef = new EnvelopeDefinition
            {
                TemplateId = body.DocuSignTemplateId,
                EmailSubject = body.Subject,
                TemplateRoles = PopulateRoles(body),
                Status = STATUS_SENT                                    // triggers a send 
            };
            return envelopeDef;
        }
        private List<TemplateRole> PopulateRoles(SignatureRequest signatureRequest)
        {
            TemplateRole role = new TemplateRole();

            role.Email = signatureRequest.EmailAddress;
            role.Name = signatureRequest.FirstName + " " + signatureRequest.LastName;
            role.RoleName = SIGNER_ROLE;
            role.Tabs = new Tabs();
            role.Tabs.TextTabs = FormatTextTabs(signatureRequest.Fields);

            return new List<TemplateRole>() { role };
        }
        private List<Text> FormatTextTabs(Dictionary<string, string> fields)    
        {
            List<Text> textTabs = new List<Text>();
            foreach (KeyValuePair<string, string> field in fields)
            {
                Text textField = new Text
                {
                    RecipientId = RECIPIENT_ID,
                    Name = field.Key,
                    TabLabel = "\\*" + field.Key,           // special juju : DocuSign pre-text for updating tabs
                    Value = field.Value,
                };
                textTabs.Add(textField);
            }
            return textTabs;
        }
        private Response FormatExceptionResponse(Exception exc)
        {
            Response response = new Response();
            response.Meta.Add("status", "exception");
            response.Meta.Add("hresult", exc.HResult.ToString());
            response.Meta.Add("exception-message", exc.ToString());
            if( exc.InnerException != null)
            {
                response.Meta.Add("inner-exception", exc.InnerException.ToString());
            }
            return response;
        }
        private Response FormatEnvelopeResponse(EnvelopeSummary envelopeSummary)
        {
            Response response = new Response();
            response.Meta.Add("status", envelopeSummary.Status);
            response.Data.Add("id", envelopeSummary.EnvelopeId);               // Service's   Id == EnvelopeId
            return response;
        }
        private void SetAuthHeader(string username, string password, string key)
        {
            Configuration.Default.ApiClient = _apiClient;
            if (Configuration.Default.DefaultHeader.ContainsKey(key))
            {
                Configuration.Default.DefaultHeader.Remove(key);
            }
            Configuration.Default.AddDefaultHeader("X-DocuSign-Authentication", "{\"Username\":\"" + username + "\", \"Password\":\"" + password + "\", \"IntegratorKey\":\"" + key + "\"}");
        }
        private void GetConfigSettings(out string retApiUrl, out string retIntegrationKey)
        {
            IConfiguration configuration;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            configuration = builder.Build();

            retApiUrl = configuration["ApiUrl"];
            retIntegrationKey = configuration["IntegrationKey"];
        }
    }
}
