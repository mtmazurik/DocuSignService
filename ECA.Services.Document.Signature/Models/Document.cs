using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECA.Services.Document.Signature.Models
{
    public class Document
    {
        public string Name { get; set; }
        public string DocumentBase64 { get; set; }
    }
}
