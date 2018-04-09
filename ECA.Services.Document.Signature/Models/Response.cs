using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECA.Services.Document.Signature.Models
{
    public class Response
    {
        // usage:     Response resp = new Response();
        //
        //            resp.Meta.Add("status","ok");
        //
        //            resp.Data.Add("id", "12345678-9876-3456-1234-546780424789");

        private Dictionary<string, string> _meta;
        private Dictionary<string, string> _data;

        public Response()           // ctor
        {
            _meta = new Dictionary<string, string>();
            _data = new Dictionary<string, string>();
        }

        public Dictionary<string,string> Meta {  get { return _meta; } }

        public Dictionary<string,string> Data {  get { return _data; } }

    }
}
