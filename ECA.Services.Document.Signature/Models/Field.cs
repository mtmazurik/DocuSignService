using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECA.Services.Document.Signature.Models
{
    public class Field
    {
        public string Name { get; set; }
        public string FieldType { get; set; }           // json for "string", "bool"  currently supported
        public string DataType { get; set; }
        public string Value { get; set; }
    }
}
