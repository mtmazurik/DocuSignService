using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECA.Services.Document.Signature.Models
{
    public class SignatureRequest
    {
        public string DocuSignUsername { get; set; }
        public string DocuSignPassword { get; set; }
        public string DocuSignTemplateId { get; set; }
        public string EmailAddress { get; set; }
        public string Subject { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Fields { get; set; }
    }

}
