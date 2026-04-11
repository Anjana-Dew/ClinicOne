using ClinicOne.Data;
using ClinicOne.Models.Entities;

namespace ClinicOne.Services
{
    public class AccessLogService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccessLogService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public void Log(string patientNic, string action)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var userId = httpContext.Session.GetInt32("UserID");

            if(userId == null)
            {
                return;
            }

            var doctor = _context.Doctors.FirstOrDefault(d => d.UserAccountID == userId);

            if (doctor == null) 
            {
                return;
            }
            var log = new AccessLog
            {
                DoctorID =doctor.DoctorID,
                PatientNIC = patientNic,
                Action = action,
                AccessDateTime = DateTime.Now,
            };

            _context.AccessLogs.Add(log);
            _context.SaveChanges();
        }
    }
}
