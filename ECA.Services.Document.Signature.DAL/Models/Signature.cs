using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Linq;

namespace ECA.Services.Document.Signature.DAL.Models
{
    [Table("Signature")]
    public partial class Signature
    {
        public int SignatureId { get; set; }
        public string Status { get; set; }
        public DateTime UTCDateTimeCreated { get; set; }
        public DateTime UTCDateTimeLastUpdated { get; set; }
        public Guid TemplateId { get; set; }
        public Guid EnvelopeId { get; set; }
        public string DocuSignUsername { get; set; }
        public string DocuSignPassword { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
    }
}
