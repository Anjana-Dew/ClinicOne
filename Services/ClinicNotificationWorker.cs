using ClinicOne.Models.Entities;
using Microsoft.Extensions.Hosting;

public class ClinicNotificationWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    public ClinicNotificationWorker(IServiceProvider services) => _services = services;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ClinicOne.Data.ApplicationDbContext>();

                // 1. Scan for reports missing from Notification table
                var newReports = context.MedicalReports
                    .Where(r => !context.Notifications.Any(n => n.Message.Contains("RPT:" + r.ReportID)))
                    .ToList();

                foreach (var r in newReports)
                {
                    context.Notifications.Add(new Notification
                    {
                        PatientNIC = r.PatientNIC,
                        Message = "[REPORT] New medical report available RPT:" + r.ReportID,
                        SentDate = DateTime.Now,
                        IsRead = false
                    });
                }

                await context.SaveChangesAsync();
            }
            await Task.Delay(10000, stoppingToken); // Checks every 10 seconds
        }
    }
}