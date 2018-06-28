using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ECA.Services.Document.Signature.Models;
using Newtonsoft.Json.Linq;

namespace ECA.Services.Document.Signature.DocuSign
{
    public interface IRestApiWrapper
    {
        string GetAccountId(string username, string password);
        HttpStatusCode SendDocuSignRequest(HttpMethod method, string uri, string username, string password, out JObject returnContent, JObject optionalBodyContent = null);
    }
}