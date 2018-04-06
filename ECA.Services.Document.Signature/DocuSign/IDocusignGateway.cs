using DocuSign.eSign.Api;
using ECA.Services.Document.Signature.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECA.Services.Document.Signature.DocuSign
{
    public interface IDocuSignGateway
    {
        Response Send(Models.SignatureRequest request);

    }
}
