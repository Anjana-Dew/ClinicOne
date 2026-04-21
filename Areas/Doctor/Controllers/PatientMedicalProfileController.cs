using ClinicOne.Data;
using ClinicOne.Models.ViewModels.Doctor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicOne.Services;
using System.Diagnostics;
using ClinicOne.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicOne.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class PatientMedicalProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AccessLogService _accessLogService;


        public PatientMedicalProfileController(ApplicationDbContext context, AccessLogService accessLogService)
        {
            _context = context;
            _accessLogService = accessLogService;
        }
        

    }
}
