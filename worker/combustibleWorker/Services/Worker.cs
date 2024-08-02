using combustibleWorker.AppSettingModels;
using combustibleWorker.Interface;
using combustibleWorker.Models;
using Microsoft.Extensions.Options;

namespace combustibleWorker.Services
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration configuration;
        private readonly Schedule schedule;

        private Timer _timer;
        private readonly ICombustibleService combServ;
        private readonly IDataRepository repo;
        private readonly UrlPage urlPage;
        private readonly XPathExpression xpath;
        private readonly WaitingTimeMilisecond waitingTimeMilisecond;
        private readonly OnFailureRetryQty onFailureRetryQty;
        public Worker(
            ILogger<Worker> logger,
            IConfiguration configuration,
            IOptions<UrlPage> urlPage,
            IOptions<XPathExpression> xpath,
            IOptions<WaitingTimeMilisecond> WaitingTimeMilisecond,
            IOptions<OnFailureRetryQty> onFailureRetry,
            IOptions<Schedule> schedule,
            ICombustibleService _combServ,
            IDataRepository _repo
            )
        {
            _logger = logger;
            this.configuration = configuration;
            this.schedule = schedule.Value;
            onFailureRetryQty = onFailureRetry.Value;
            combServ = _combServ;
            repo = _repo;
            this.urlPage = urlPage.Value;
            this.xpath = xpath.Value;
            waitingTimeMilisecond = WaitingTimeMilisecond.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (isToday() && isNow())
                {
                    syncAsync(stoppingToken);
                }
                int waiting = Convert.ToInt32(waitingTimeMilisecond.WaitingTime);
                _logger.LogInformation("Worker running at: {time} each {kk} miliseconds", DateTimeOffset.Now, waiting.ToString());
                _logger.LogInformation("Service will Sync on {0} at {1}", schedule.day,schedule.hour);
                _logger.LogInformation("Current day is {0} at {1}", DateTime.Now.DayOfWeek, DateTime.Now.Hour);
                await Task.Delay(waiting, stoppingToken);
            }
        }
        protected bool isToday()
        {
            DayOfWeek today = new DayOfWeek();
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
                    throw new ArgumentException("Invalid day of the week");
            }

            bool is2day = currentTime.DayOfWeek == today;
            
            return is2day;
        }

        protected bool isNow()
        {
            var currentTime = DateTime.Now;
            bool isNow = currentTime.Hour == Convert.ToInt32(schedule.hour);
            return isNow;
        }

        protected async Task syncAsync(CancellationToken stoppingToken)
        {
            
                string waiting = waitingTimeMilisecond.WaitingTime;
                int rety = Convert.ToInt32(onFailureRetryQty.OnFailureRetry);
                
                bool completed = false;
                int intent = 0;
                while (!completed && intent < rety)
                {
                    List<Combustible> info = combServ.GetCombustible();

                foreach (Combustible n in info)
                {
                    var response = await repo.AddAsync(n);
                }
                if (info.Any())
                    {
                        completed = true;
                    }
                    else
                    {
                        intent++;
                    _logger.LogInformation("({0}) No se pudo completar la sincronizacion, reintento " + intent.ToString(),DateTime.Now);
                    }
                }
        }
    }
}
