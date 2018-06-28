using ECA.Services.Document.Signature.Config;
using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ECA.Services.Document.Signature.Tasks
{
    internal class TaskManager : BackgroundService
    {
        private readonly IDocuSignStatusUpdater _docuSignStatusUpdater;
        private double _intervalSeconds;
        private Logger _logger;
        private IJsonConfiguration _config;

        public TaskManager(IJsonConfiguration config, IDocuSignStatusUpdater docuSignStatusUpdater)
        {
            _docuSignStatusUpdater = docuSignStatusUpdater;                             // simple task injection model
            _logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            _config = config;
            _intervalSeconds = _config.TaskManagerIntervalSeconds;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            { 
                cancellationToken.Register(() => _logger.Debug($"TaskManager background task service is stopping."));

                _logger.Info("TaskManager background thread started.");
                while (!cancellationToken.IsCancellationRequested)                                   // forever loop
                {
                    await Task.Delay(TimeSpan.FromSeconds( _intervalSeconds ), cancellationToken);   // timer
                    _logger.Debug($"awake:  isCancellationRequested: {cancellationToken.IsCancellationRequested}" );

                    await _docuSignStatusUpdater.Update();                                           // launch task
                }
                _logger.Debug($"Outside the while loop.");
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "TaskManager.ExecuteAsync error.");
            }
        }
    }
}
