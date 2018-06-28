using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using ECA.Services.Document.Signature.Config;
using ECA.Services.Document.Signature.Models;
using Models = ECA.Services.Document.Signature.Models;
using ECA.Services.Document.Signature.DocuSign.Exceptions;
using System.Net;
using ECA.Services.Document.Signature.DAL;
using DataModels = ECA.Services.Document.Signature.DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;

namespace ECA.Services.Document.Signature.DocuSign.Builders
{
    public class SignatureBuilder : ISignatureBuilder
    {
        private const string _signerRole = "signer";                                // limitation:  signer role is hard-coded
        private const string _recipientId = "1";                                    // limitation:  one recipient is hard-coded
        private const string _sent = "sent";
        private const int _initialDocumentIdValue = 10000;

        private IRestApiWrapper _api;
        private IJsonConfiguration _config;
        private IRepository _repository;
        private Logger _logger;

        public SignatureBuilder( IRestApiWrapper wrapper, IJsonConfiguration config, IRepository repository)
        {
            _api = wrapper;
            _config = config;
            _repository = repository;
            _repository.InitializeContext(_config.ConnectionString);
            _logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
        }

        public bool Build(SignatureRequest request, string username, string password, out IResponse returnResponse)
        {
            returnResponse = new Response();
            try
            {
                JArray returnDataArray = new JArray();

                foreach (string emailAddress in request.EmailAddresses)
                {
                    SendEnvelope(emailAddress, request, username, password, returnDataArray);
                    string numberAttachedDocuments = request.Documents == null ? "0" : request.Documents.Length.ToString();
                    _logger.Info($"Signature request sent: email {emailAddress}, Attached docs: {numberAttachedDocuments}");
                }
                returnResponse.Data = returnDataArray;
                return true;
            }
            catch (Exception exc)   
            {
                _logger.Error(exc,"Build()");
                returnResponse.InsertExceptionIntoMeta(exc);
                return false;
            }
        }

        private void SendEnvelope(string emailAddress, SignatureRequest request, string username, string password, JArray returnDataArray)
        {
            string accountId = _api.GetAccountId(username, password);
            string uri = $"{_config.ApiUrl}/v2/accounts/{accountId}/envelopes";

            if (request.Documents != null) // with attached documents
            {
                CreateEnvelopeFromTemplate(emailAddress, request, username, password, returnDataArray, uri, request.Documents);
            }
            else
            {
                CreateEnvelopeFromTemplate(emailAddress, request, username, password, returnDataArray, uri);  
            }
        }

        private void CreateEnvelopeFromTemplate(string emailAddress, SignatureRequest request, string username, string password, JArray returnDataArray, string uri, Models.Document[] documents = null)
        {
            JObject returnContent;
            HttpStatusCode status = _api.SendDocuSignRequest(HttpMethod.Post, uri, username, password, out returnContent, FormatCreateEnvelopeBody(_sent, emailAddress, request, documents));
            if (status != HttpStatusCode.Created)
            {
                throw new SigSvcDocuSignAPICallFailure("Error creating envelope.  Check URI and body of Signature request.");
            }
            else
            {
                string envelopeId = returnContent.SelectToken(@"envelopeId").ToString();
                FillReturnDataArray(returnDataArray, envelopeId, emailAddress, returnContent);
                WriteToRepository(envelopeId, username, password, _sent, request, returnContent);
            }
        }

        private void WriteToRepository(string envelopeId, string username, string password, string status, SignatureRequest request, JObject returnContent)
        {
            _repository.CreateSignature(new DataModels.Signature        // save to repository
            {
                Status = status,                            // "sent", "created" = draft (not sent immediately)
                UTCDateTimeCreated = DateTime.UtcNow,
                UTCDateTimeLastUpdated = DateTime.UtcNow,
                TemplateId = Guid.Parse(request.DocuSignTemplateId),
                EnvelopeId = Guid.Parse(envelopeId),
                DocuSignUsername = username,
                DocuSignPassword = password,
                RequestBody = JsonConvert.SerializeObject(request, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                ResponseBody = returnContent.ToString()
            });
        }

        private static void FillReturnDataArray(JArray returnDataArray, string envelopeId, string emailAddress, JObject returnContent)
        {
            var data = new JObject();
            data.Add("email", emailAddress);
            data.Add("envelopeId", envelopeId);
            data.Add("statusDateTime", returnContent.SelectToken(@"statusDateTime").ToString());
            returnDataArray.Add(data);
        }
        private JObject FormatCreateEnvelopeBody(string envelopeStatusType, string emailAddress, SignatureRequest request, Models.Document[] documents)       
        {
            JObject envelopeDefinition = new JObject
            {
                { "status", envelopeStatusType },
                { "templateId", request.DocuSignTemplateId },
                { "emailSubject", request.Subject },
                { "name", request.Name },
                { "templateRoles", BuildRoles(emailAddress, request) },
                { "compositeTemplates", BuildCompositeTemplates(emailAddress, request.Name, documents) }
            };
            return envelopeDefinition;
        }
        private JArray BuildCompositeTemplates(string emailAddress, string name, Models.Document[] documents)
        {
            if (documents is null)  // no attached documents
                return null;
            int computedDocumentId = _initialDocumentIdValue;
            JArray compositeTemplates = new JArray();

            foreach (Models.Document document in documents)
            {
                JArray signers = new JArray()
                {
                    new JObject()
                    {
                        new JProperty("recipientId", "1"),
                        new JProperty("email", emailAddress),
                        new JProperty("name", name),
                        new JProperty("defaultRecipient", "true")
                    }
                };
                JArray inlineTemplates = new JArray()
                {
                    new JObject()
                    {
                        new JProperty("sequence", "1"),
                        new JProperty("recipients", new JObject() { new JProperty("signers", signers)})
                    }
                };
                computedDocumentId++;
                JObject documentObject = new JObject
                {
                    new JProperty("documentId", computedDocumentId),
                    new JProperty("name", document.Name),
                    new JProperty("transformPdfFields", true),
                    new JProperty("documentBase64", document.DocumentBase64),
                };
                JObject compositeTemplate = new JObject()
                {
                    new JProperty("inlineTemplates", inlineTemplates),
                    new JProperty("document", documentObject)
                };
                compositeTemplates.Add(compositeTemplate);
            }
            return compositeTemplates;
        }
        private JArray BuildRoles(string emailAddress, SignatureRequest request)
        {
            JArray array = new JArray();

            JObject roleObject = new JObject
            { 
                new JProperty("email", emailAddress),     
                new JProperty("name", request.Name),
                new JProperty("roleName", _signerRole),
            };
            if ( request.Fields != null )
            {
                roleObject.Add(new JProperty("tabs", this.BuildTabs(request.Fields)));
            }
            array.Add(roleObject);
            return (array);
        }

        private JObject BuildTabs(Field[] fields)
        {
            Dictionary<string, JArray> tabArrays = new Dictionary<string, JArray>();
            JObject tabsObject = new JObject();
           
            foreach (Field field in fields)                             
            {
                JArray tabArray = GetTabArray(tabArrays, field);
                string valueType = GetValueType(field);

                JObject fieldObject = new JObject
                {
                    new JProperty("recipientId", _recipientId),
                    new JProperty("tabLabel", $"\\*{field.Name}"),      // wildcard \\*   find all occurances of tabLabel in document
                    new JProperty(valueType, field.Value)
                };
                tabArray.Add(fieldObject);
            };
            foreach(  KeyValuePair<string,JArray>item in tabArrays)
            {
                tabsObject.Add(new JProperty(item.Key, item.Value));   // now, create named json arrays to stuff into JObject and return 
            }
            return tabsObject;
        }

        private string GetValueType(Field field)
        {
            string valueType = "value";
            if ( String.Compare( field.FieldType.ToString(), "Checkbox", StringComparison.OrdinalIgnoreCase) == 0 )
            {
                valueType = "selected";                             // for checkbox
            }

            return valueType;
        }

        private JArray GetTabArray(Dictionary<string, JArray> tabArrays, Field field)
        {
            string tabGroupName = field.FieldType + "Tabs";         // example:  'NameTabs', 'EmailTabs', etc.

            JArray tabArray = null;
            if ( tabArrays.ContainsKey(tabGroupName) != true)       // see if we already have one
            {
                tabArray = new JArray();                            // create a new one
                tabArrays.Add(tabGroupName, tabArray);
            }
            else   // get existing one
            {
                tabArrays.TryGetValue(tabGroupName, out tabArray);
            }
            return tabArray;
        }
    }

    public interface ISignatureBuilder                              // interface for injection
    {
        bool Build(SignatureRequest request, string username, string password, out IResponse response);
    }
}

