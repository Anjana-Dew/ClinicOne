using ClinicOne.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicOne.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Core Users
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Pharmacist> Pharmacists { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }

        // Medical Reports
        public DbSet<MedicalReport> MedicalReports { get; set; }
        public DbSet<ReportTestResult> ReportTestResults { get; set; }
        public DbSet<TestType> TestTypes { get; set; }
        public DbSet<TestRange> TestRanges { get; set; }

        // Prescriptions
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionMedicine> PrescriptionMedicines { get; set; }
        public DbSet<ExternalPrescription> ExternalPrescriptions { get; set; }
        public DbSet<PrescribedTest> PrescribedTests { get; set; }


        // Progress Tracking
        public DbSet<PatientProgress> PatientProgresses { get; set; }

        // Scheduling
        public DbSet<ClinicSession> ClinicSessions { get; set; }
        public DbSet<ClinicSchedule> ClinicSchedules { get; set; }
        public DbSet<DoctorSchedule> DoctorDutySchedules { get; set; }

        // Notifications & Logs
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AccessLog> AccessLogs { get; set; }
        public DbSet<MedicineReminder> MedicineReminders { get; set; }

 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>().ToTable("Patient");
            modelBuilder.Entity<Doctor>().ToTable("Doctor");
            modelBuilder.Entity<Admin>().ToTable("Admin");
            modelBuilder.Entity<Pharmacist>().ToTable("Pharmacist");
            modelBuilder.Entity<Prescription>().ToTable("Prescription");
            modelBuilder.Entity<MedicalReport>().ToTable("MedicalReport");
            modelBuilder.Entity<TestType>().ToTable("TestType");
            modelBuilder.Entity<TestRange>().ToTable("TestRange");
            modelBuilder.Entity<ReportTestResult>().ToTable("ReportTestResult");
            modelBuilder.Entity<PrescriptionMedicine>().ToTable("PrescriptionMedicine");
            modelBuilder.Entity<ClinicSession>().ToTable("ClinicSession");
            modelBuilder.Entity<ClinicSchedule>().ToTable("ClinicSchedule");
            modelBuilder.Entity<Notification>().ToTable("Notification");
            modelBuilder.Entity<PatientProgress>().ToTable("PatientProgress");
            modelBuilder.Entity<MedicineReminder>().ToTable("MedicineReminder");
            modelBuilder.Entity<ExternalPrescription>().ToTable("ExternalPrescription");
            modelBuilder.Entity<PrescribedTest>().ToTable("PrescribedTest");
            modelBuilder.Entity<DoctorSchedule>().ToTable("DoctorSchedule");
            modelBuilder.Entity<AccessLog>().ToTable("AccessLog");
            modelBuilder.Entity<UserAccount>().ToTable("UserAccount");
            



            base.OnModelCreating(modelBuilder);

            // USER ACCOUNT
            modelBuilder.Entity<UserAccount>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<UserAccount>()
                .HasCheckConstraint("CK_UserAccount_Role",
                    "[Role] IN ('Patient','Doctor','Admin','Pharmacist')");

            modelBuilder.Entity<UserAccount>()
                .Property(u => u.IsLocked)
                .HasDefaultValue(false);

            modelBuilder.Entity<UserAccount>()
                .Property(u => u.FailedAttempts)
                .HasDefaultValue(0);

            // PATIENT
            modelBuilder.Entity<Patient>()
        .       HasCheckConstraint("CK_Patient_Gender",
                "Gender IN ('M','F')");

            modelBuilder.Entity<Patient>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Patient>()
                .Property(p => p.Height)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Patient>()
                .Property(p => p.Weight)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Patient>()
                .HasOne(p => p.UserAccount)
                .WithOne(u => u.patient)
                .HasForeignKey<Patient>(p => p.UserAccountID)
                .OnDelete(DeleteBehavior.Restrict);

            // DOCTOR
            modelBuilder.Entity<Doctor>()
                .Property(d => d.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.UserAccount)
                .WithOne()
                .HasForeignKey<Doctor>(d => d.UserAccountID)
                .OnDelete(DeleteBehavior.Restrict);

            // ADMIN
            modelBuilder.Entity<Admin>()
                .Property(a => a.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Admin>()
                .HasOne(a => a.UserAccount)
                .WithOne()
                .HasForeignKey<Admin>(a => a.UserAccountID)
                .OnDelete(DeleteBehavior.Restrict);

            // PHARMACIST
            modelBuilder.Entity<Pharmacist>()
                .HasOne(p => p.UserAccount)
                .WithOne()
                .HasForeignKey<Pharmacist>(p => p.UserAccountID)
                .OnDelete(DeleteBehavior.Restrict);

            // MEDICAL REPORT (NO CASCADE TO PATIENT)
            modelBuilder.Entity<MedicalReport>()
                .Property(m => m.UploadedDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<MedicalReport>()
                .HasOne(m => m.Patient)
                .WithMany(p => p.MedicalReports)
                .HasForeignKey(m => m.PatientNIC)
                .OnDelete(DeleteBehavior.Restrict);

            // TEST RANGE
            modelBuilder.Entity<TestRange>()
                .HasOne(r => r.TestType)
                .WithOne(t => t.TestRange)
                .HasForeignKey<TestRange>(r => r.TestTypeID)
                .OnDelete(DeleteBehavior.Cascade);

            // REPORT TEST RESULT
            modelBuilder.Entity<ReportTestResult>()
                .HasCheckConstraint("CK_ReportTestResult_Status",
                "ResultStatus IN ('Normal','High','Risk')");

            modelBuilder.Entity<ReportTestResult>()
                .Property(r => r.TestValue)
                .HasPrecision(10, 2);

            modelBuilder.Entity<ReportTestResult>()
                .HasOne(r => r.MedicalReport)
                .WithMany(m => m.ReportTestResults)
                .HasForeignKey(r => r.ReportID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReportTestResult>()
                .HasOne(r => r.TestType)
                .WithMany(t => t.ReportTestResults)
                .HasForeignKey(r => r.TestTypeID)
                .OnDelete(DeleteBehavior.Restrict);

            // PRESCRIPTION
            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Patient)
                .WithMany(p => p.Prescriptions)
                .HasForeignKey(p => p.PatientNIC)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PrescriptionMedicine>()
                .HasCheckConstraint("CK_PrescriptionMedicine_Status",
                    "[Status] IN ('Given','Not Given','Partially Given')");

            modelBuilder.Entity<PrescriptionMedicine>()
                .Property(p => p.PatientConfirmed)
                .HasDefaultValue(false);

            modelBuilder.Entity<PrescriptionMedicine>()
                .HasOne(p => p.Prescription)
                .WithMany(p => p.PrescriptionMedicines)
                .HasForeignKey(p => p.PrescriptionID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExternalPrescription>()
                .Property(e => e.GeneratedDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<ExternalPrescription>()
                .HasOne(e => e.Prescription)
                .WithOne(p => p.ExternalPrescription)
                .HasForeignKey<ExternalPrescription>(e => e.PrescriptionID)
                .OnDelete(DeleteBehavior.Cascade);

            // PRESCRIBED TEST
            modelBuilder.Entity<PrescribedTest>()
                .HasOne(p => p.Prescription)
                .WithMany(p => p.PrescribedTests)
                .HasForeignKey(p => p.PrescriptionID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PrescribedTest>()
                .HasOne(p => p.TestType)
                .WithMany(t => t.PrescribedTests)
                .HasForeignKey(p => p.TestTypeID)
                .OnDelete(DeleteBehavior.Restrict);

            // CLINIC SCHEDULE
            modelBuilder.Entity<ClinicSchedule>()
                .Property(c => c.AssignedDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<ClinicSchedule>()
                .HasOne(c => c.Patient)
                .WithMany(p => p.ClinicSchedules)
                .HasForeignKey(c => c.PatientNIC)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClinicSchedule>()
                .HasOne(c => c.ClinicSession)
                .WithMany(s => s.ClinicSchedules)
                .HasForeignKey(c => c.SessionID)
                .OnDelete(DeleteBehavior.Restrict);

            // NOTIFICATION
            modelBuilder.Entity<Notification>()
                .Property(n => n.SentDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Notification>()
                .Property(n => n.IsRead)
                .HasDefaultValue(false);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Patient)
                .WithMany(p => p.Notifications)
                .HasForeignKey(n => n.PatientNIC)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.ClinicSchedule)
                .WithMany(c => c.Notifications)
                .HasForeignKey(n => n.ScheduleID)
                .OnDelete(DeleteBehavior.Restrict);

            // PATIENT PROGRESS
            modelBuilder.Entity<PatientProgress>()
                .HasCheckConstraint("CK_PatientProgress_Status",
                "ProgressStatus IN ('Improving','Stable','Worsening')");

            modelBuilder.Entity<PatientProgress>()
                .Property(p => p.IsConfirmed)
                .HasDefaultValue(false);

            modelBuilder.Entity<PatientProgress>()
                .Property(p => p.RecordedDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<PatientProgress>()
                .HasOne(p => p.Patient)
                .WithMany(p => p.PatientProgresses)
                .HasForeignKey(p => p.PatientNIC)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PatientProgress>()
                .HasOne(p => p.MedicalReport)
                .WithMany(m => m.PatientProgresses)
                .HasForeignKey(p => p.ReportID)
                .OnDelete(DeleteBehavior.Restrict);

            // MEDICINE REMINDER
            modelBuilder.Entity<MedicineReminder>()
                .Property(m => m.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<MedicineReminder>()
                .HasOne(m => m.Patient)
                .WithMany(p => p.MedicineReminders)
                .HasForeignKey(m => m.PatientNIC)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MedicineReminder>()
                .HasOne(m => m.PrescriptionMedicine)
                .WithMany(p => p.MedicineReminders)
                .HasForeignKey(m => m.PrescMedID)
                .OnDelete(DeleteBehavior.Cascade);

            // DOCTOR SCHEDULE
            modelBuilder.Entity<DoctorSchedule>()
                .HasOne(d => d.Doctor)
                .WithMany(d => d.DoctorSchedules)
                .HasForeignKey(d => d.DoctorID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DoctorSchedule>()
                .HasOne(d => d.ClinicSession)
                .WithMany(c => c.DoctorSchedules)
                .HasForeignKey(d => d.SessionID)
                .OnDelete(DeleteBehavior.Restrict);

            // ACCESS LOG
            modelBuilder.Entity<AccessLog>()
                .HasCheckConstraint("CK_AccessLog_Action",
                 "[Action] IN ('View','Update','Prescribe')");

            modelBuilder.Entity<AccessLog>()
                .Property(a => a.AccessDateTime)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<AccessLog>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.AccessLogs)
                .HasForeignKey(a => a.PatientNIC)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AccessLog>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.AccessLogs)
                .HasForeignKey(a => a.DoctorID)
                .OnDelete(DeleteBehavior.Restrict);

        }


        // all the setters and getters to function our programs. for now just ignore this. and DON'T dare to touch this without my permisisions.... 
    }
}
