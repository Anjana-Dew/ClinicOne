using ClinicOne.Data;
using ClinicOne.Models.ViewModels.Doctor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClinicOne.Services;
using System.Diagnostics;
using ClinicOne.Models.Entities;

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
        public IActionResult Index(string id)
        {
            // vitals and personal info
            var patient = _context.Patients.FirstOrDefault(p => p.PatientNIC == id);

            //Vitals
            var vitals = _context.PatientVitals
                .Where(v => v.PatientNIC == id)
                .OrderByDescending(v => v.RecordedDate)
                .ToList();

            var latestHeight = vitals.FirstOrDefault(v => v.Height != null)?.Height;
            var latestWeight = vitals.FirstOrDefault(v => v.Weight != null)?.Weight;
            var latestBp = vitals.FirstOrDefault(v => v.Systolic != null && v.Diastolic != null);

            string bloodPressure = latestBp != null ? $"{latestBp.Systolic}/{latestBp.Diastolic}" : null;

            if (patient == null)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            _accessLogService.Log(id, "View");

            decimal? bmi = null;


            if (latestHeight != null && latestWeight != null) { 
                
                var heightM = (decimal)latestHeight / 100;
                bmi = latestWeight / (heightM * heightM);
            }

            var model = new PatientMedicalProfileViewModel
            {
                PatientNIC = patient.PatientNIC,
                FullName = patient.FullName,
                BloodType = patient.BloodType,
                Address = patient.Address,
                PhoneNumber = patient.PhoneNumber,
                Height = latestHeight,
                Weight = latestWeight,
                BloodPressure = bloodPressure,
                BMI = bmi
            };
            

            // test reports
            var reports = _context.MedicalReports
                            .Where(r => r.PatientNIC == id)
                            .OrderByDescending(r => r.UploadedDate)
                            .ToList();

            Debug.WriteLine($"Report Count:{reports.Count}");

            var reportCards = new List<MedicalReportCardViewModel>();

            foreach (var report in reports)
            {
                Debug.WriteLine($"Processing reportID: {report.ReportID} Date: {report.UploadedDate}");
                var results = (from rtr in _context.ReportTestResults
                               join tp in _context.TestParameters on rtr.ParameterID equals tp.ParameterID
                               join panel in _context.TestPanels on tp.PanelID equals panel.PanelID
                               where rtr.ReportID == report.ReportID
                               select new
                               {
                                   panel.PanelID,
                                   panel.TestName,
                                   tp.ParameterName,
                                   tp.Unit,
                                   rtr.TestValue,
                                   rtr.ResultStatus
                               }).ToList();
                Debug.WriteLine($"Paramteres found: {results.Count}");
                var panelId = results.FirstOrDefault()?.PanelID;
                var panelName = results.FirstOrDefault()?.TestName;
                Debug.WriteLine($"Panel : {panelId}");

                if (panelId == null)
                    continue;

                if (reportCards.Any(c => c.PanelName == panelName))
                    continue;

                var latestResults = results.Select(r => new ParameterResultViewModel
                {
                    ParameterName = r.ParameterName,
                    TestValue = r.TestValue,
                    Unit = r.Unit,
                    ResultStatus = r.ResultStatus
                }).ToList();

                var previousReport = (from mr in _context.MedicalReports
                                      join rtr in _context.ReportTestResults on mr.ReportID equals rtr.ReportID
                                      join tp in _context.TestParameters on rtr.ParameterID equals tp.ParameterID
                                      where mr.PatientNIC == id
                                      && mr.ReportID != report.ReportID
                                      && mr.UploadedDate < report.UploadedDate
                                      && tp.PanelID == panelId
                                      select mr)
                                      .Distinct()
                                      .OrderByDescending(r => r.UploadedDate)
                                      .ThenByDescending(r => r.ReportID)
                                      .FirstOrDefault();

                List<ParameterResultViewModel> previousResults = null;
                string previousPath = null;
                DateTime? previousDate = null;

                if (previousReport != null)
                {
                    Debug.WriteLine($"Previous report found: {previousReport.ReportID}");
                    previousResults = (from rtr in _context.ReportTestResults
                                       join tp in _context.TestParameters on rtr.ParameterID equals tp.ParameterID
                                       where rtr.ReportID == previousReport.ReportID
                                       select new ParameterResultViewModel
                                       {
                                           ParameterName = tp.ParameterName,
                                           TestValue = rtr.TestValue,
                                           Unit = tp.Unit,
                                           ResultStatus = rtr.ResultStatus
                                       }).ToList();

                    previousPath = previousReport.ReportPath;
                    previousDate = previousReport.UploadedDate;
                }
                else
                {
                    Debug.WriteLine($"no previous report found");
                }

                reportCards.Add(new MedicalReportCardViewModel
                {
                    PanelName = panelName,
                    LatestUploadedDate = report.UploadedDate,
                    LatestReportPath = report.ReportPath,
                    LatestResults = latestResults,
                    PreviousUploadedDate = previousDate,
                    PreviousReportPath = previousPath,
                    PreviousResults = previousResults
                });

                if (reportCards.Count == 2)
                    break;
            }

            model.MedicalReports = reportCards;

            //progress calculation

            var latestReportDate = _context.MedicalReports
                .Where(r => r.PatientNIC == id)
                .Max(r => (DateTime?)r.ReportDate);

            var previousReportDate = _context.MedicalReports
                .Where(r => r.PatientNIC == id && r.ReportDate < latestReportDate)
                .Max(r => (DateTime?)r.ReportDate);

            if (latestReportDate != null && previousReportDate != null)
            {
                var latestResults = (from rtr in _context.ReportTestResults
                                     join mr in _context.MedicalReports
                                     on rtr.ReportID equals mr.ReportID
                                     where mr.PatientNIC == id &&
                                           mr.ReportDate == latestReportDate
                                     select rtr)
                    .ToList();

                var previousResults = (from rtr in _context.ReportTestResults
                                       join mr in _context.MedicalReports
                                       on rtr.ReportID equals mr.ReportID
                                       where mr.PatientNIC == id &&
                                             mr.ReportDate == previousReportDate
                                       select rtr)
                    .ToList();

                var suggested = CalculateProgress(latestResults, previousResults);

                var existing = _context.PatientProgresses
                                .FirstOrDefault(p => p.PatientNIC == id && p.ProgressDate == latestReportDate.Value);

                if(existing == null)
                {
                    var newProgress = new PatientProgress
                    {
                        PatientNIC = id,
                        ProgressDate = latestReportDate.Value,
                        ProgressStatus = suggested,
                        IsConfirmed = false,
                        RecordedDate = DateTime.Now
                    };

                    _context.PatientProgresses.Add(newProgress);
                    _context.SaveChanges();

                    existing = newProgress;
                }

                model.Progress = new PatientProgressViewModel
                {
                    PatientNIC = id,
                    ProgressDate = latestReportDate.Value,
                    SuggestedStatus = suggested,
                    CurrentStatus = existing?.ProgressStatus?? suggested,
                    IsConfirmed = existing?.IsConfirmed?? false,
                    DoctorNotes = existing?.DoctorNotes
                };
            }

            //Medicine histories

            var prescriptions = _context.Prescriptions.
                Where(p => p.PatientNIC == id)
                .OrderByDescending(p => p.PrescriptionDate)
                .ThenByDescending(p =>p.PrescriptionID)
                .Take(2)
                .ToList();

            var medicineCards = new List<MedicineHistoryCardViewModel>();

            foreach(var pres in prescriptions)
            {
                var meds = _context.PrescriptionMedicines
                    .Where(m => m.PrescriptionID == pres.PrescriptionID)
                    .Select(m => new MedicineItemViewModel
                    {
                        MedicineName = m.MedicineName,
                        Dosage = m.Dosage,
                        TimesPerDay = m.TimesPerDay
                    }).ToList();

                var nextClinic = _context.ClinicSchedules
                    .Where(c => c.PatientNIC == id && c.ClinicDate > pres.PrescriptionDate)
                    .OrderBy(c => c.ClinicDate)
                    .FirstOrDefault();

                medicineCards.Add(new MedicineHistoryCardViewModel
                {
                    PrescriptionDate = pres.PrescriptionDate,
                    UntilDate = nextClinic?.ClinicDate,
                    Medicines = meds
                });
            }
            model.MedicineHistories = medicineCards;

            //Clinic Scheduling

            model.ClinicSessions = _context.ClinicSessions
                .Select(s => new ClinicSessionItemViewModel
                {
                    SessionID = s.SessionID,
                    SessionName = s.SessionName,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    RemainingSlots = s.MaxSlots - _context.ClinicSchedules
                    .Count(c => c.SessionID == s.SessionID && c.ClinicDate == DateTime.Today)
                }).ToList();

            return View(model);
        }

        // update vital methods
        [HttpPost]
        public IActionResult UpdateHeight([FromBody] HeightUpdateRequest request)
        {
            var patient = _context.Patients.FirstOrDefault(p => p.PatientNIC == request.Nic);

            if(patient == null)
            {
                return NotFound();
            }

            var vital = new PatientVital
            {
                PatientNIC = request.Nic,
                Height = request.Height
            };
            
            _context.PatientVitals.Add(vital);    
            _context.SaveChanges();

            _accessLogService.Log(request.Nic, "Update");

            TempData["Success"] = "Height updated successfully";

            return Ok();
        }

        [HttpPost]
        public IActionResult UpdateWeight([FromBody] WeightUpdateRequest request) 
        {
            var patient = _context.Patients.FirstOrDefault(p => p.PatientNIC == request.Nic);

            if (patient == null)
            {
                return NotFound();
            }
            var vital = new PatientVital
            {
                PatientNIC = request.Nic,
                Weight = request.Weight
            };

            _context.PatientVitals.Add(vital);

            _context.SaveChanges();

            _accessLogService.Log(request.Nic, "Update");

            TempData["Success"] = "Weight updated successfully";

            return Ok();
        }
        [HttpPost]
        public IActionResult UpdateBP([FromBody] BPUpdateRequest request)
        {
            var patient = _context.Patients.FirstOrDefault(p => p.PatientNIC == request.Nic);

            if (patient == null)
            {
                return NotFound();
            }

            int? systolic = null;
            int? diastolic = null;

            if(!string.IsNullOrEmpty(request.Bp) && request.Bp.Contains("/")) 
            {
                var parts = request.Bp.Split('/');

                if(parts.Length == 2)
                {
                    systolic = int.Parse(parts[0]);
                    diastolic= int.Parse(parts[1]);
                }
            }
            var vital = new PatientVital
            {
                PatientNIC = request.Nic,
                Systolic = systolic,
                Diastolic = diastolic
            };

            _context.PatientVitals.Add(vital);
            _context.SaveChanges();

            _accessLogService.Log(request.Nic, "Update");

            TempData["Success"] = "Blood Pressure updated successfully";
            return Ok();
        }

        [HttpPost]
        public IActionResult UpdateBloodType([FromBody] BloodTypeRequest request)
        {
            var patient = _context.Patients.FirstOrDefault(p => p.PatientNIC == request.Nic);

            if (patient == null)
                return NotFound();

            if (patient.BloodType != null)
                return BadRequest();

            patient.BloodType = request.BloodType;

            _context.SaveChanges();

            _accessLogService.Log(request.Nic, "Update");

            return Ok();
        }

        [HttpPost]
        public IActionResult ScheduleClinic(string PatientNIC, DateTime ClinicDate, int SelectedSessionID)
        {
            var session = _context.ClinicSessions
                .FirstOrDefault(s => s.SessionID == SelectedSessionID);

            if(session == null)
            {
                return RedirectToAction("Index", new { id = PatientNIC });
            }

            var bookedCount = _context.ClinicSchedules
                .Count(c => c.SessionID == SelectedSessionID && c.ClinicDate == ClinicDate);

            if(bookedCount >= session.MaxSlots)
            {
                TempData["Error"] = "No more available slots for this clinic session.";
                return RedirectToAction("Index", new { id = PatientNIC });
            }

            var alreadyBooked = _context.ClinicSchedules
                .Any(c => c.PatientNIC == PatientNIC && c.ClinicDate == ClinicDate);

            if (alreadyBooked)
            {
                TempData["Error"] = "Patient already has a clinic appointment on this date.";
                return RedirectToAction("Index", new { id = PatientNIC });
            }
            var schedule = new ClinicSchedule
            {
                PatientNIC = PatientNIC,
                ClinicDate = ClinicDate,
                SessionID = SelectedSessionID
            };

            _context.ClinicSchedules.Add(schedule);
            _context.SaveChanges();

            TempData["Success"] = "Next clinic scheduled successfully.";

            return RedirectToAction("Index", new { id = PatientNIC });
        }

        public IActionResult GetSessionsForDate(DateTime clinicDate)
        {
            var sessions = _context.ClinicSessions
                .Select(s => new
                {
                    s.SessionID,
                    s.SessionName,
                    s.StartTime,
                    s.EndTime,
                    s.MaxSlots,
                    Booked = _context.ClinicSchedules
                    .Count(c => c.SessionID == s.SessionID && c.ClinicDate == clinicDate)
                })
                .ToList()
                .Select(s => new
                {
                    s.SessionID,
                    s.SessionName,
                    s.StartTime,
                    s.EndTime,
                    RemainingSlots = clinicDate.Date == DateTime.Today && s.StartTime <= DateTime.Now.TimeOfDay
                    ? 0 : s.MaxSlots - s.Booked
                });

            return Json(sessions);
        }

        private string CalculateProgress(List<ReportTestResult> latestResults, List<ReportTestResult> previousResults)
        {
            int normalPrev = 0, highPrev = 0, riskPrev = 0;
            int normalCurr = 0, highCurr = 0, riskCurr = 0;

            foreach (var r in previousResults)
            {
                if (r.ResultStatus == "Normal") normalPrev++;
                if (r.ResultStatus == "High") highPrev++;
                if (r.ResultStatus == "Risk") riskPrev++;
            }

            foreach (var r in latestResults)
            {
                if (r.ResultStatus == "Normal") normalCurr++;
                if (r.ResultStatus == "High") highCurr++;
                if (r.ResultStatus == "Risk") riskCurr++;
            }

            if(riskCurr < riskPrev || highCurr < highPrev)
            {
                return "Improving";
            }

            if(riskCurr > riskPrev)
            {
                return "Worsening";
            }
            return "Stable";
        }

        [HttpPost]
        public IActionResult ConfirmProgress(string patientNIC, DateTime progressDate, string doctorNotes, string SuggestedStatus)
        {
            var progress = _context.PatientProgresses
                .FirstOrDefault(p => p.PatientNIC == patientNIC && p.ProgressDate == progressDate);
            if(progress == null)
            {
                progress = new PatientProgress
                {
                    PatientNIC = patientNIC,
                    ProgressDate = progressDate,
                    RecordedDate = DateTime.Now
                };
                _context.PatientProgresses.Add(progress);
            }

            progress.ProgressStatus = SuggestedStatus ?? "Stable";
            progress.DoctorNotes = doctorNotes;
            progress.IsConfirmed = true;

            _context.SaveChanges();

            TempData["Success"] = "Progress confirmed.";
            return RedirectToAction("Index", new { id = patientNIC });
        }

        //public IActionResult UpdateProgressStatus(string patientNIC, DateTime progressDate, string progressStatus, string doctorNotes)
        //{
        //    var progress = _context.PatientProgresses.FirstOrDefault(p => p.PatientNIC == patientNIC && p.ProgressDate == progressDate);

        //    if (progress != null)
        //    {
        //        progress.ProgressStatus = progressStatus;
        //        progress.DoctorNotes = doctorNotes;

        //        _context.SaveChanges();
        //    }

        //    TempData["Success"] = "Progress updated.";

        //    return RedirectToAction("Index", new { id = patientNIC });
        //}
    }
}
