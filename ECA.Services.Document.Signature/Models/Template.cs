using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECA.Services.Document.Signature.Models
{
    public class Template
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public List<Field> Fields { get; set; }

        //public List<Document> Documents { get; set; }          
    }
}
