using System;
using System.Threading;
using System.Threading.Tasks;
using Application.IServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundServices
{
    public class TaskAutoCompleteBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TaskAutoCompleteBackgroundService> _logger;
        private readonly TimeSpan _period = TimeSpan.FromMinutes(5); // Check every 5 minutes

        public TaskAutoCompleteBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<TaskAutoCompleteBackgroundService> logger)
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
                    using var scope = _serviceProvider.CreateScope();
                    var taskService = scope.ServiceProvider.GetRequiredService<ITaskService>();

                    var result = await taskService.AutoCompleteExpiredTasksAsync();

                    if (result.Success && result.Data > 0)
                    {
                        _logger.LogInformation($"Auto-completed {result.Data} expired tasks");
                    }
                    else if (!result.Success)
                    {
                        _logger.LogError($"Error auto-completing tasks: {result.Message}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in TaskAutoCompleteBackgroundService");
                }

                await Task.Delay(_period, stoppingToken);
            }
        }
    }
}