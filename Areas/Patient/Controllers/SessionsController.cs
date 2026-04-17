using ClinicOne.Data;
using ClinicOne.Models.ViewModels.Patient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicOne.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class SessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        
       
    }
}