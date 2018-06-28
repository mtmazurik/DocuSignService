using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace ECA.Services.Document.Signature.Models
{
    public interface IResponse
    {
        Dictionary<string, string> Meta { get; }
        Object Data { set;  get; }
        void InsertStatusCodeIntoMeta(int statusCode);
        void InsertExceptionIntoMeta(Exception exc);
    }
}