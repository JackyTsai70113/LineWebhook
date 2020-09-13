using System;
using System.Threading;
using System.Threading.Tasks;
using BL.Services.Line;
using Microsoft.Extensions.Logging;

namespace BL.Services.HostedService {

    public class NotifyCronJobService : BaseCronJobService {
        private readonly LineNotifyBotService _lineNotifyBotService;
        private readonly ILogger<NotifyCronJobService> _logger;

        public NotifyCronJobService(
            IScheduleConfig<NotifyCronJobService> config, ILogger<NotifyCronJobService> logger)
        : base(config.CronExpression, config.TimeZoneInfo) {
            _logger = logger;
            _lineNotifyBotService = new LineNotifyBotService(); ;
        }

        public override Task StartAsync(CancellationToken cancellationToken) {
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWorkAsync(CancellationToken cancellationToken) {
            _lineNotifyBotService.PushMessage_Jacky($"{DateTime.Now:HH:mm:ss fff} KD通知即將上線!");
            _lineNotifyBotService.PushMessage_Jacky($"https://tinyurl.com/y5ou8l6a");
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken) {
            _logger.LogInformation("NotifyCronJobService is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}