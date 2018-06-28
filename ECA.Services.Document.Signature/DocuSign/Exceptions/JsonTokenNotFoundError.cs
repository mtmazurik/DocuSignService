using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECA.Services.Document.Signature.DocuSign.Exceptions
{
    public class JsonTokenNotFoundError : Exception
    {
        public JsonTokenNotFoundError() { }
        public JsonTokenNotFoundError(string message) :
            base(message) { }   
    }
}
