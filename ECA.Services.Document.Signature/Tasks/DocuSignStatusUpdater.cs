using ECA.Services.Document.Signature.Config;
using ECA.Services.Document.Signature.DAL;
using DataModels = ECA.Services.Document.Signature.DAL.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECA.Services.Document.Signature.DocuSign.Builders;
using ECA.Services.Document.Signature.Models;

namespace ECA.Services.Document.Signature.Tasks
{
    public class DocuSignStatusUpdater : IDocuSignStatusUpdater
    {
        private Logger _logger;
        private IJsonConfiguration _config;
        private IRepository _repo;
        private IStatusBuilder _statusBuilder;
        public DocuSignStatusUpdater(IJsonConfiguration config, IRepository repo, IStatusBuilder statusBuilder)                 //ctor
        {
            _logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            _config = config;
            _repo = repo;
            _repo.InitializeContext(_config.ConnectionString);
            _statusBuilder = statusBuilder;
        }

        public async Task Update()
        {
            try
            {
                await Task.Run(() => CheckSignatureStatuses());
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "DocuSignStatusUpdater.Update() thread error.");
            }
        }

        private void CheckSignatureStatuses()
        {

            // todo: need thread safety here ?   Will race condition occur? critical section?  Reading all of them, updating existing ones?

            // read all signatures from DB
            List<DataModels.Signature> signatures = _repo.ReadAllSignatures();
            foreach( DataModels.Signature signature in signatures)
            {
                if( signature.Status != "completed" &
                    signature.Status != "declined" &
                    signature.Status != "voided" &
                    signature.Status != "expired" )
                { 
                    IResponse response;
                    string envelopeId = signature.EnvelopeId.ToString();
                    _statusBuilder.Build(envelopeId, signature.DocuSignUsername, signature.DocuSignPassword, out response);   // get current DocuSign status
                    string currentStatus = ((EnvelopeStatus)response.Data).Status;
                    if ( !(string.Compare( currentStatus, signature.Status ) == 0 ) )   // if status is different
                    {
                        _logger.Debug($"Updating status:  from: {signature.Status} to: {currentStatus} EnvelopeId {envelopeId} ");
                        signature.Status = currentStatus;
                        _repo.UpdateSignature(signature);                               // save current status to database 

                    }
                }
            }

            // end critical section ?
        }
    }
}
