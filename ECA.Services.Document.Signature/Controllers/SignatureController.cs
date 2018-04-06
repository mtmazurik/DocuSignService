using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECA.Services.Document.Signature.Models;
using ECA.Services.Document.Signature.DocuSign;
using Microsoft.AspNetCore.Mvc;
using DocuSign.eSign.Api;


namespace ECA.Services.Document.Signature.Controllers
{
    [Route("signature/")]
    public class SignatureController : Controller
    {

        [HttpGet("{guid}/fields")]
        [ProducesResponseType(200)]
        public IActionResult Get(Guid? guid)
        {
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(200)]
        public IActionResult Post([FromBody]SignatureRequest body)         // POST signature   { json body == SignatureRequest } 
        {
            IDocuSignGateway gateway = new DocuSignGateway( new AuthenticationApi() as IAuthenticationApi, new EnvelopesApi() as EnvelopesApi ) as IDocuSignGateway;
            return Ok( gateway.Send(body) );
        }

        [HttpGet("ping")]
        [ProducesResponseType(200)]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
