using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BackgroundTasksSample.Services
{
    #region snippet1
    internal class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;

        public TimedHostedService(ILogger<TimedHostedService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");


                DateTime nd = DateTime.Now;
                int hh = nd.Hour;
                int mm = nd.Minute;
                int start = 11;
                int du = (start - hh) * 1000 * 60 * 60 - mm * 1000 * 60;
                int p = 86400000;
                if (du < 0)
                {
                    du = du + p;
                }

                _timer = new Timer(
                    DoWork, null, du, p);

            
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");
            try
            {
                System.Diagnostics.Process batch = new System.Diagnostics.Process();
                batch.StartInfo.FileName = @"C:\Projects2018\task.cmd";
                batch.Start();
            }
            catch
            {; }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
    #endregion
}