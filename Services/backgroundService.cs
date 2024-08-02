using Microsoft.Extensions.Hosting;

namespace combustiblesRDBack.Services
{
    public class BService: BackgroundService
    {
        public BService(ILogger<BackgroundService> logger)
        {
            
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
