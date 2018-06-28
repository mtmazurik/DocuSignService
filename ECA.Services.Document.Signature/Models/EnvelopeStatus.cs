using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECA.Services.Document.Signature.Models
{
    public class EnvelopeStatus
    {
        private string _status;
        public EnvelopeStatus(string status)
        {
            _status = status;
        }
        public string Status
        {
            get { return _status; }
        }
    }
}
