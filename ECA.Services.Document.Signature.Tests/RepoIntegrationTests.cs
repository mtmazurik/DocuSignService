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
using DataModels = ECA.Services.Document.Signature.DAL.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ECA.Services.Document.Signature.Tests
{
    [TestClass]
    public class RepoIntegrationTests
    {
        IRepository _repo;
        DataModels.Signature _preFabSignature;

        [TestInitialize]
        public void Initialize()
        {
            IJsonConfiguration config = new JsonConfiguration();
            _repo = new Repository();
            _repo.InitializeContext(config.ConnectionString);
            _preFabSignature = new DataModels.Signature();
            _preFabSignature.EnvelopeId = Guid.Parse("4a39ce48-cdcd-466d-9c68-4b7c8529a211");
            _preFabSignature.TemplateId = Guid.Parse("4a39ce48-cdcd-466d-9c68-4b7c8529a211");
            _preFabSignature.RequestBody = "{" +
                "  \"docuSignUsername\": \"slewarne @epiqsystems.com\"," +
                "  \"docuSignPassword\": \"P@ssword1\",  " +
                "  \"docuSignTemplateId\": \"4a39ce48-cdcd-466d-9c68-4b7c8529a211\"," +
                "  \"emailAddress\": [\"mmazurik@epiqglobal.com\",\"martymazurik@gmail.com\"]," +
                "  \"subject\": \"DocuSign document is ready to sign\"," +
                "  \"name\" : \"Marty Mazurik\"," +
                "  \"fields\": [" +
                "        {" +
                "            \"name\": \"Text c649cc4c-29b8-4a3a-bbc0-691775d8a693\"," +
                "            \"fieldType\": \"Text\"," +
                "            \"dataType\": \"string\"," +
                "            \"value\" : \"some text\"" +
                "        }," +
                "        {" +
                "            \"name\": \"Note 970f97e8-272e-41ab-9801-b5bef15e120c\"," +
                "            \"fieldType\": \"Note\"," +
                "            \"dataType\": \"string\"," +
                "            \"value\": \"Note, duly noted.\"" +
                "        }," +
                "        {" +
                "            \"name\": \"Checkbox 0360f6d2-eab3-4da6-8623-45aa693fac17\"," +
                "            \"fieldType\": \"Checkbox\"," +
                "            \"dataType\": \"boolean\"," +
                "            \"value\": \"true\"" +
                "        }" +
                "]}";
            _preFabSignature.ResponseBody = "{" +
                "    \"meta\": {" +
                "            }," +
                "    \"data\": [" +
                "        {" +
                "            \"email\": \"mmazurik@epiqglobal.com\"," +
                "            \"envelopeId\": \"0cc930d8-9604-45cf-92c4-f8cad93bcca7\"," +
                "            \"statusDateTime\": \"05/08/2018 20:49:20\"" +
                "        }" +
                "    ]}";
            _preFabSignature.Status = "Sent";

        }
        [IgnoreAttribute]
        [TestMethod]
        public void RepoGetSingleSignatureTest()
        {
           DataModels.Signature foundOne = _repo.ReadSignature(1);  
        }
        [IgnoreAttribute]
        [TestMethod]
        public void RepoGetAllSignaturesTest()
        {
            List<DataModels.Signature> signatures = _repo.ReadAllSignatures();
        }
        [IgnoreAttribute]
        [TestMethod]
        public void RepoCreateSignatureTest()
        {
            _repo.CreateSignature(_preFabSignature);
        }
        [IgnoreAttribute]
        [TestMethod]
        public void RepoUpdateSignatureTest()
        {
            _preFabSignature.Status = "Completed";      // change state of db record to 'Completed'
            _preFabSignature.SignatureId = 2;           // which one we are referring to
            _repo.UpdateSignature(_preFabSignature);
        }

    }
}
