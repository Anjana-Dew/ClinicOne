using ClinicOne.Data;
using ClinicOne.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public class NotificationInterceptor
{
    private readonly ApplicationDbContext _context;

    public NotificationInterceptor(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task HandleNotifications()
    {
        var entries = _context.ChangeTracker.Entries().ToList();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                await HandleAdded(entry);
            }

            if (entry.State == EntityState.Modified)
            {
                await HandleModified(entry);
            }
        }
    }

    private async Task HandleAdded(EntityEntry entry)
    {
        switch (entry.Entity.GetType().Name)
        {
            case "MedicalReport":
                var report = entry.Entity as MedicalReport;

                await AddNotification(
                    report.PatientNIC,
                    "New medical report added"
                );
                break;

            case "ExternalPrescription":
                var ext = entry.Entity as ExternalPrescription;

                var nic = _context.Prescriptions
                    .Where(p => p.PrescriptionID == ext.PrescriptionID)
                    .Select(p => p.PatientNIC)
                    .FirstOrDefault();

                await AddNotification(
                    nic,
                    "External prescription added. View PDF"
                );
                break;

            case "PatientProgress":
                var prog = entry.Entity as PatientProgress;

                await AddNotification(
                    prog.PatientNIC,
                    "Your health progress updated"
                );
                break;
        }
    }

    private async Task HandleModified(EntityEntry entry)
    {
        if (entry.Entity.GetType().Name == "PrescriptionMedicine")
        {
            var med = entry.Entity as PrescriptionMedicine;

            // Pharmacist processed
            if (!string.IsNullOrEmpty(med.Status))
            {
                var nic = _context.Prescriptions
                    .Where(p => p.PrescriptionID == med.PrescriptionID)
                    .Select(p => p.PatientNIC)
                    .FirstOrDefault();

                await AddNotification(
                    nic,
                    "Your prescription has been processed"
                );
            }
        }
    }

    private async Task AddNotification(string nic, string msg)
    {
        if (string.IsNullOrEmpty(nic)) return;

        _context.Notifications.Add(new Notification
        {
            PatientNIC = nic,
            Message = msg,
            SentDate = DateTime.Now,
            IsRead = false
        });

        await _context.SaveChangesAsync();
    }
}