using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BL.Services.HostedService {

    public abstract class BaseCronJobService : IHostedService, IDisposable {
        private System.Timers.Timer _timer;
        private readonly CronExpression _expression;
        private readonly TimeZoneInfo _timeZoneInfo;

        public BaseCronJobService(string cronExpression, TimeZoneInfo timeZoneInfo) {
            _expression = CronExpression.Parse(cronExpression);
            _timeZoneInfo = timeZoneInfo;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <remarks>
        /// 執行ScheduleJobAsync
        /// StartAsync contains the logic to start the background task. StartAsync is called before:
        /// The app's request processing pipeline is configured (Startup.Configure).
        /// The server is started and IApplicationLifetime.ApplicationStarted is triggered.
        /// </remarks>
        public virtual Task StartAsync(CancellationToken cancellationToken) {
            return ScheduleJobAsync(cancellationToken);
        }

        protected virtual async Task ScheduleJobAsync(CancellationToken cancellationToken) {
            var next = _expression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);
            if (next.HasValue) {
                var delay = next.Value - DateTimeOffset.Now;
                if (delay.TotalMilliseconds <= 0)   // prevent non-positive values from being passed into Timer
                {
                    await ScheduleJobAsync(cancellationToken);
                }
                _timer = new System.Timers.Timer(delay.TotalMilliseconds);
                _timer.Elapsed += async (sender, args) => {
                    _timer.Dispose();  // reset and dispose timer
                    _timer = null;

                    if (!cancellationToken.IsCancellationRequested) {
                        await DoWorkAsync(cancellationToken);
                    }

                    if (!cancellationToken.IsCancellationRequested) {
                        await ScheduleJobAsync(cancellationToken);    // reschedule next
                    }
                };
                _timer.Start();
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// 可被override執行真正要做的事情
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task DoWorkAsync(CancellationToken cancellationToken) {
            return Task.Delay(5000, cancellationToken);  // do the work
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <remarks>
        /// 停止timer
        /// Triggered when the host is performing a graceful shutdown.
        /// StopAsync contains the logic to end the background task.
        /// Implement IDisposable and finalizers (destructors) to dispose of any unmanaged resources.
        /// </remarks>
        public virtual Task StopAsync(CancellationToken stoppingToken) {
            _timer?.Stop();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 讓timer的資源釋放，以便garbage collection
        /// </summary>
        public void Dispose() {
            _timer?.Dispose();
        }
    }

    public interface IScheduleConfig<T> {
        string CronExpression { get; set; }
        TimeZoneInfo TimeZoneInfo { get; set; }
    }

    public class ScheduleConfig<T> : IScheduleConfig<T> {
        public string CronExpression { get; set; }
        public TimeZoneInfo TimeZoneInfo { get; set; }
    }

    public static class ScheduledServiceExtensions {

        public static IServiceCollection AddCronJob<T>(
            this IServiceCollection services, Action<IScheduleConfig<T>> options) where T : BaseCronJobService {
            if (options == null) {
                throw new ArgumentNullException(nameof(options), @"Please provide Schedule Configurations.");
            }
            var config = new ScheduleConfig<T>();
            options.Invoke(config);
            if (string.IsNullOrWhiteSpace(config.CronExpression)) {
                throw new ArgumentNullException(nameof(ScheduleConfig<T>.CronExpression), @"Empty Cron Expression is not allowed.");
            }

            services.AddSingleton<IScheduleConfig<T>>(config);
            services.AddHostedService<T>();
            return services;
        }
    }
}