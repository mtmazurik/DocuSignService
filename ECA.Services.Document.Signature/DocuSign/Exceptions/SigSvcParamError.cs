using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECA.Services.Document.Signature.DocuSign.Exceptions
{
    public class SigSvcParamError : Exception
    {
        public SigSvcParamError(string message) : 
            base(message)
        {
        }   
    }
}
