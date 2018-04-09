using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECA.Services.Document.Signature.Models;
using ECA.Services.Document.Signature.DocuSign;
using Microsoft.AspNetCore.Mvc;
using DocuSign.eSign.Api;
using System.Net;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ECA.Services.Document.Signature.Controllers
{

    /// <summary>
    /// Signature Service
    /// </summary>
    [Route("signature/")]
    public class SignatureController : Controller
    {
        [HttpGet("{docuSignTemplate-guid}/fields")]
        [ProducesResponseType(200)]
        [ProducesResponseType(501)]
        public IActionResult Get(Guid? guid)
        {
            return StatusCode(501);  // nyi
        }
        [HttpPost]
        [ProducesResponseType(200)]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(Response))]
        public IActionResult Post([FromBody]SignatureRequest body, [FromServices]IDocuSignGateway gateway )         // aspnetcore inject gateway, see: startup.cs
        {
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
