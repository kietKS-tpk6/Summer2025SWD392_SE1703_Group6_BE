using Application.IServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class TestAutoApprovalService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TestAutoApprovalService> _logger;

        public TestAutoApprovalService(IServiceProvider serviceProvider, ILogger<TestAutoApprovalService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var testService = scope.ServiceProvider.GetRequiredService<ITestService>();
                        await testService.ProcessAutoApprovalAsync();
                    }

                    _logger.LogInformation("Auto approval process completed at: {time}", DateTimeOffset.Now);

                    // Run every hour
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in auto approval service");
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                }
            }
        }
    }
}