using ClinicOne.Data; // your DbContext namespace
using ClinicOne.Models.ViewModels.Patient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicOne.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        
    }
}

