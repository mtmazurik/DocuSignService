using System.Collections.Generic;
using ECA.Services.Document.Signature.DAL.Models;

namespace ECA.Services.Document.Signature.DAL
{
    public interface IRepository
    {
        void CreateSignature(Models.Signature newSignature);
        List<Models.Signature> ReadAllSignatures();
        Models.Signature ReadSignature(int signatureId);
        void UpdateSignature(Models.Signature updatedSignature);

        void InitializeContext(string connection);
    }
}