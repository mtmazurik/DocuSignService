using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECA.Services.Document.Signature.DocuSign.Exceptions
{
    public class SigSvcDocuSignAPICallFailure : Exception
    {
        public SigSvcDocuSignAPICallFailure(string message) : 
            base(message)
        {
        }   
    }
}
