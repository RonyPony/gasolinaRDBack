using combustibleWorker.AppSettingModels;
using combustibleWorker.Interface;
using combustibleWorker.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace combustibleWorker.Services
{
    public class Worker : BackgroundService
    {
        private readonly IConfiguration configuration;
        private readonly Schedule schedule;
        private readonly ICombustibleService combServ;
        private readonly IDataRepository repo;
        private readonly UrlPage urlPage;
        private readonly XPathExpression xpath;
        private readonly WaitingTimeMilisecond waitingTimeMilisecond;
        private readonly OnFailureRetryQty onFailureRetryQty;

        public Worker(
            IConfiguration configuration,
            IOptions<UrlPage> urlPage,
            IOptions<XPathExpression> xpath,
            IOptions<WaitingTimeMilisecond> waitingTimeMilisecond,
            IOptions<OnFailureRetryQty> onFailureRetry,
            IOptions<Schedule> schedule,
            ICombustibleService combServ,
            IDataRepository repo
            )
        {
            this.configuration = configuration;
            this.schedule = schedule.Value;
            this.onFailureRetryQty = onFailureRetry.Value;
            this.combServ = combServ;
            this.repo = repo;
            this.urlPage = urlPage.Value;
            this.xpath = xpath.Value;
            this.waitingTimeMilisecond = waitingTimeMilisecond.Value;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.Log("Worker starting...");
            // Log all configuration values to ensure they are loaded correctly                        
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.Log("Worker Started at: " + DateTimeOffset.Now);
            Logger.Log("Starting Service Health Check...");
            verifyIntegrity(stoppingToken);
            if (schedule.syncOnStartup)
            {
                Logger.Log("Sync On Startup Enabled");
                Logger.Log("Sync has started");
                await syncAsync(stoppingToken);
                Logger.Log("Closing Sync process");
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                if (isToday() && isNow())
                {
                    Logger.Log("Sync has started");
                    await syncAsync(stoppingToken);
                    Logger.Log("Closing Sync process");
                }

                int waiting = Convert.ToInt32(waitingTimeMilisecond.WaitingTime);
                Logger.Log("Worker running at: " + DateTimeOffset.Now + " each " + waiting.ToString() + " milliseconds");
                Logger.Log("Service will Sync on " + schedule.day + " at " + schedule.hour);
                Logger.Log("Current day is " + DateTime.Now.DayOfWeek + " at " + DateTime.Now.Hour);

                await Task.Delay(waiting, stoppingToken);
            }
        }

        private void verifyIntegrity(CancellationToken tk)
        {
            int err = 0;
            if (string.IsNullOrEmpty(schedule.day))
            {
                Logger.Log("Scheduled day not set");
                err++;
            }
            if (string.IsNullOrEmpty(schedule.hour))
            {
                Logger.Log("Scheduled hour not set");
                err++;
            }
            if (string.IsNullOrEmpty(waitingTimeMilisecond.WaitingTime))
            {
                Logger.Log("waitingTimeMilisecond not set");
                err++;
            }
            if (string.IsNullOrEmpty(onFailureRetryQty.OnFailureRetry))
            {
                Logger.Log("OnFailureRetry not set");
                err++;
            }
            if (string.IsNullOrEmpty(urlPage.Url))
            {
                Logger.Log("Url not set");
                err++;
            }
            if (string.IsNullOrEmpty(xpath.XPath))
            {
                Logger.Log("XPath not set");
                err++;
            }
            Logger.Log("Service Health Check Results");
            if (err==0)
            {
                Logger.Log("All Tests Completed, Ready to operate.");
            }
            else
            {
                Logger.Log(err.ToString()+" Error, please configure the service and provide the correct configuration");
                tk.ThrowIfCancellationRequested();
            }
        }

        protected bool isToday()
        {
            DayOfWeek today = DayOfWeek.Monday; // Default to Monday
            var currentTime = DateTime.Now;
            switch (schedule.day.ToLower().Trim())
            {
                case "monday":
                    today = DayOfWeek.Monday;
                    break;
                case "tuesday":
                    today = DayOfWeek.Tuesday;
                    break;
                case "wednesday":
                    today = DayOfWeek.Wednesday;
                    break;
                case "thursday":
                    today = DayOfWeek.Thursday;
                    break;
                case "friday":
                    today = DayOfWeek.Friday;
                    break;
                case "saturday":
                    today = DayOfWeek.Saturday;
                    break;
                case "sunday":
                    today = DayOfWeek.Sunday;
                    break;
                default:
                    Logger.Log("Invalid day of the week");
                    break;
            }

            return currentTime.DayOfWeek == today;
        }

        protected bool isNow()
        {
            var currentTime = DateTime.Now;
            return currentTime.Hour == Convert.ToInt32(schedule.hour);
        }

        protected async Task syncAsync(CancellationToken stoppingToken)
        {
            string waiting = waitingTimeMilisecond.WaitingTime;
            int retry = Convert.ToInt32(onFailureRetryQty.OnFailureRetry);

            bool completed = false;
            int attempt = 0;

            while (!completed && attempt < retry && !stoppingToken.IsCancellationRequested)
            {
                List<Combustible> info = combServ.GetCombustible();

                foreach (Combustible n in info)
                {
                    var response = await repo.AddAsync(n);
                }

                if (info.Any())
                {
                    completed = true;
                    const string space = "##########################################";
                    Logger.Log(space);
                    Logger.Log("Sync Completed");
                    Logger.Log(space);
                }
                else
                {
                    attempt++;
                    Logger.Log("No se pudo completar la sincronización, reintento " + attempt.ToString());
                    await Task.Delay(Convert.ToInt32(waiting), stoppingToken);
                }
            }
        }
    }
}
