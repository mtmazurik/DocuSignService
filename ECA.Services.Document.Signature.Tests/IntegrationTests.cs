using Microsoft.VisualStudio.TestTools.UnitTesting;
using ECA.Services.Document.Signature.DocuSign.Exceptions;
using ECA.Services.Document.Signature.DocuSign;
using ECA.Services.Document.Signature.Config;
using Newtonsoft.Json.Linq;
using System;
using ECA.Services.Document.Signature.Models;
using System.Net.Http;
using ECA.Services.Document.Signature.DocuSign.Builders;
using ECA.Services.Document.Signature.DAL;

namespace ECA.Services.Document.Signature.Tests
{
    [TestClass]
    public class IntegrationTests
    {  
        IRestApiWrapper _api;
        IJsonConfiguration _config;
        IRepository _repository;

        string _username = "slewarne@epiqsystems.com";
        const string _password = "P@ssword1";
        const string _template_TestTemplate = "00e571b0-a6e8-4709-aaad-3b50f82bdcbb";
        const string _template_ManyFields = "4a39ce48-cdcd-466d-9c68-4b7c8529a211";
        const string _W4_Template = "f5589daf-b74d-430d-8822-851cdecc1a75";

        [TestInitialize]
        public void Initialize()
        {
            _api = new RestApiWrapper(new HttpClient(), new JsonConfiguration());
            _config = new JsonConfiguration();
            _repository = new Repository();
        }

        [IgnoreAttribute]
        [TestMethod]
        public void GetEnvelopStatusTest()
        {
            IStatusBuilder builder = new StatusBuilder(_api, new JsonConfiguration());
            IResponse response;
            Assert.IsTrue(builder.Build("5058294f-25d3-4da4-9cb7-926d95dc66b1", _username, _password, out response));
            Assert.AreEqual("completed", ((EnvelopeStatus)response.Data).Status);
        }

        [IgnoreAttribute]
        [TestMethod]
        //[ExpectedException(typeof(ConfigFileReadError))]
        public void GetAccountIdTest()
        {
            IRestApiWrapper wrapper = new RestApiWrapper(new System.Net.Http.HttpClient(), new JsonConfiguration());
            _username = "mmazurik@epiqglobal.com";
            string response = wrapper.GetAccountId(_username, _password);
            Assert.AreEqual("4823252", response);
        }

        [IgnoreAttribute]
        [TestMethod]
        public void GetTemplateFieldsTest()
        {
            ITemplateFieldBuilder builder = new TemplateFieldBuilder(_api, new JsonConfiguration());
            IResponse response;
            Assert.IsTrue(builder.Build(_template_ManyFields, _username, _password, out response));
            Assert.AreEqual("Many Fields", ((Template)response.Data).Name);
        }

        [IgnoreAttribute]
        [TestMethod]
        public void PostSignatureTest()
        {
            Field[] fields = {
                    new Field
                    {
                        Name = "caseName",
                        Value = "Hollywood Mogul vs. Oil Shotgun Well",
                        DataType = "string",
                        FieldType = "text"
                    },
                    new Field
                    {
                        Name = "claimantName",
                        Value = "Jed Clampett",
                        DataType = "string",
                        FieldType = "text"
                    }
                };

            SignatureRequest request = new SignatureRequest
            {
                DocuSignTemplateId = _W4_Template,
                EmailAddresses = new string[] { "mmazurik@epiqglobal.com" },
                Subject = "SigSvc Integration Test Send. Your Document is ready to sign",
                Name = "Marty Mazurik",
                Fields = fields
            };

            ISignatureBuilder builder = new SignatureBuilder(_api, _config, null);      // fix test _repo would need to be set up
            IResponse response;
            Assert.IsTrue(builder.Build(request, "mmazurik@epiqglobal.com", _password, out response));
        }
    }
}
