using ECA.Services.Document.Signature.Config;
using ECA.Services.Document.Signature.Models;
using Models = ECA.Services.Document.Signature.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ECA.Services.Document.Signature.JsonHelpers;
using ECA.Services.Document.Signature.DocuSign.Exceptions;
using System.Net;
using NLog;

namespace ECA.Services.Document.Signature.DocuSign.Builders
{
    public class TemplateFieldBuilder : ITemplateFieldBuilder
    {
        private const int _signersIndex = 0;          // signers, limitation: 1 right now, called  "Signer"

        private IRestApiWrapper _api;
        private IJsonConfiguration _config;
        private Logger _logger;

        public TemplateFieldBuilder(IRestApiWrapper wrapper, IJsonConfiguration config)
        {
            _api = wrapper;                     // ctor
            _config = config;
            _logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
        }
        public bool Build(string templateId, string username, string password, out IResponse returnResponse)
        {
            returnResponse = new Response();
            try
            {
                string accountId = _api.GetAccountId(username, password);

                string uri = $"{_config.ApiUrl}/v2/accounts/{accountId}/templates/{templateId}";

                JObject returnedContent;
                HttpStatusCode status = _api.SendDocuSignRequest(HttpMethod.Get, uri, username, password, out returnedContent);
                if (status != HttpStatusCode.OK)
                {
                    string message = "Get Document Fields failed. Check TemplateID passed in querystring and retry.";
                    _logger.Error($"Build() {message}");
                    throw new SigSvcTemplateIdFailure(message);
                }

                AddTemplateTabsToResponse(returnedContent, returnResponse);
                return true;
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Build()");
                returnResponse.InsertExceptionIntoMeta(exc);
                return false;
            }
        }
        private void AddTemplateTabsToResponse(JObject joTemplate, IResponse returnResponse)
        {
            Template template = GetTemplateInfo(joTemplate);

            template.Fields = ExtractTemplateFields(joTemplate) ;

            returnResponse.Data = template;
        }

        List<Field> ExtractTemplateFields(JObject templateDef)
        {
            List<Field> retFields = new List<Field>();

            JObject recipients = new JObject(templateDef.Value<JObject>("recipients"));
            JArray signers= recipients["signers"].Value<JArray>();
            JObject signer = signers[_signersIndex].Value<JObject>();
            JObject tabs = signer.Value<JObject>("tabs");

            AddSpecificTabs(tabs, "text", "string", retFields);           // supported Tab groups (our FieldType)
            AddSpecificTabs(tabs, "note", "string", retFields);
            AddSpecificTabs(tabs, "checkbox", "bool", retFields);

            return retFields;
        }

        private void AddSpecificTabs(JObject allTabs, string tabType, string dataType, List<Field> fieldList)
        {
            string tabSpecificString = tabType + "Tabs";
            JArray specificTabs = allTabs.Value<JArray>(tabSpecificString);

            if( specificTabs != null)
            { 
                foreach (JObject tab in specificTabs)
                {
                    string tabLabel = tab.Value<string>("tabLabel");
                    Field field = fieldList.Find(item => item.Name == tabLabel);    // de-dup; only add if we don't have one already
                    if (field == null)
                    {
                        fieldList.Add(new Field
                        {
                            Name = tabLabel,
                            FieldType = tabType,
                            DataType = dataType
                        });
                    }
                }
            }
        }
        private Template GetTemplateInfo(JObject joTemplate)
        {
            JObject templateDef = joTemplate.Value<JObject>("envelopeTemplateDefinition");
            Template template = new Template();
            template.Id = templateDef.Value<string>("templateId");
            template.Name = templateDef.Value<string>("name");
            return template;
        }
    }
    public interface ITemplateFieldBuilder              // interface - really only used for injecting
    {
        bool Build(string templateId, string username, string password, out IResponse response);
    }
}
