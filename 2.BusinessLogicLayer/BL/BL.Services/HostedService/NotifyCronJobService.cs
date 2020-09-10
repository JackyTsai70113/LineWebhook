using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BL.Services.HostedService {

    public class NotifyCronJobService : BaseCronJobService {
        private readonly ILogger<NotifyCronJobService> _logger;

        public NotifyCronJobService(IScheduleConfig<NotifyCronJobService> config, ILogger<NotifyCronJobService> logger)
        : base(config.CronExpression, config.TimeZoneInfo) {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("NotifyCronJobService starts.");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWorkAsync(CancellationToken cancellationToken) {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss fff} NotifyCronJobService is working.");
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("NotifyCronJobService is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}