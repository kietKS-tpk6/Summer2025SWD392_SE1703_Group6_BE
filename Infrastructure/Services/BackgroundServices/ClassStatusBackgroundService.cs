using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Enums;
using Infrastructure.IRepositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.BackgroundServices
{
    public class ClassStatusBackgroundService : BackgroundService
    {
        private readonly ILogger<ClassStatusBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ClassStatusBackgroundService(IServiceProvider serviceProvider, ILogger<ClassStatusBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("[BackgroundService] ClassStatusBackgroundService is running...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var configService = scope.ServiceProvider.GetRequiredService<ISystemConfigService>();

                    var timeConfig = await configService.GetConfig("class_status_update_time");

                    string updateTimeStr = "23:59"; 

                    if (timeConfig.Success && !string.IsNullOrWhiteSpace(timeConfig.Data?.Value))
                    {
                        updateTimeStr = timeConfig.Data.Value;
                    }
                    else
                    {
                        Console.WriteLine("[Warning] Missing or invalid config: class_status_update_time, using default 23:59");
                    }

                    if (!TimeSpan.TryParse(updateTimeStr, out var scheduledTime))
                    {
                        Console.WriteLine($"[Warning] Invalid time format in config: {updateTimeStr}, fallback to 23:59");
                        scheduledTime = new TimeSpan(23, 59, 0);
                    }

                    var now = DateTime.Now;
                    var targetTime = DateTime.Today.Add(scheduledTime);

                    if (now > targetTime)
                    {
                        targetTime = targetTime.AddDays(1);
                    }

                    var delay = targetTime - now;

                    Console.WriteLine($"[Delay] Sleeping until {targetTime:HH:mm:ss} to check class statuses...");
                    await Task.Delay(delay, stoppingToken);

                    await ProcessClassStatusUpdateAsync(stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("[Info] Background service canceled.");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error] Exception in delay loop: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        private async Task ProcessClassStatusUpdateAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"[Tick] Running status update at {DateTime.Now:HH:mm:ss}");

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var classRepo = scope.ServiceProvider.GetRequiredService<IClassRepository>();
                var configService = scope.ServiceProvider.GetRequiredService<ISystemConfigService>();
                var lessonService = scope.ServiceProvider.GetRequiredService<ILessonService>();
                var testEventService = scope.ServiceProvider.GetRequiredService<ITestEventService>();

                var openClasses = await classRepo.GetClassesByStatusAsync(ClassStatus.Open);
                if (!openClasses.Success || openClasses.Data == null)
                {
                    Console.WriteLine("[Warning] Failed to retrieve open classes.");
                    return;
                }

                var configResult = await configService.GetConfig("auto_approve_class_open_duration");
                if (!configResult.Success || configResult.Data == null || !int.TryParse(configResult.Data.Value, out int daysBeforeStart))
                {
                    Console.WriteLine("[Warning] Invalid or missing config: auto_approve_class_open_duration");
                    return;
                }

                foreach (var cls in openClasses.Data)
                {
                    var daysUntilStart = (cls.TeachingStartTime.Date - DateTime.Today).Days;

                    if (daysUntilStart > daysBeforeStart)
                    {
                        Console.WriteLine($"[Skip] Class {cls.ClassID} starts in {daysUntilStart} days (> {daysBeforeStart})");
                        continue;
                    }

                    var studentsClass = await classRepo.GetStudentsByClassIdAsync(cls.ClassID);
                    if (!studentsClass.Success || studentsClass.Data == null)
                    {
                        Console.WriteLine($"[Warning] Failed to get students for class {cls.ClassID}");
                        continue;
                    }

                    int activeEnrollmentCount = studentsClass.Data.Count;
                    var newStatus = activeEnrollmentCount >= cls.MinStudentAcp && activeEnrollmentCount <= cls.MaxStudentAcp
                        ? ClassStatus.Ongoing
                        : ClassStatus.Cancelled;

                    await classRepo.UpdateStatusAsync(new ClassUpdateStatusCommand
                    {
                        ClassId = cls.ClassID,
                        ClassStatus = newStatus
                    });

                    if (newStatus == ClassStatus.Cancelled)
                    {
                        Console.WriteLine($"[Action] Class {cls.ClassID} cancelled. Deleting related data...");

                        var deleteLessonResult = await lessonService.DeleteLessonByClassIDAsync(cls.ClassID);
                        if (!deleteLessonResult.Success)
                        {
                            Console.WriteLine($"[Warning] Failed to delete lessons for class {cls.ClassID}: {deleteLessonResult.Message}");
                        }

                        var deleteTestEventResult = await testEventService.DeleteTestEventsByClassIDAsync(cls.ClassID);
                        if (!deleteTestEventResult.Success)
                        {
                            Console.WriteLine($"[Warning] Failed to delete test events for class {cls.ClassID}: {deleteTestEventResult.Message}");
                        }
                    }

                    Console.WriteLine($"[Status] Class {cls.ClassID} updated to: {newStatus}");
                }

                Console.WriteLine($"[Summary] Finished processing {openClasses.Data.Count} open classes.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Exception during class status update: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
