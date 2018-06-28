using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECA.Services.Document.Signature.DocuSign.Exceptions
{
    public class SigSvcPostBodyError : Exception
    {
        public SigSvcPostBodyError(string message) : 
            base(message)
        {
        }   
    }
}
