using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECA.Services.Document.Signature.Models
{
    public class SignatureRequest
    {
        public string DocuSignTemplateId { get; set; }
        public string[] EmailAddresses { get; set; }
        public string Subject { get; set; }
        public string Name { get; set; }
        public Field[] Fields { get; set; }
        public Document[] Documents { get; set; }
    }
}
