using ClinicOne.Data;
using Microsoft.AspNetCore.Mvc;

public class BaseController : Controller
{
    protected readonly ApplicationDbContext _context;

    public BaseController(ApplicationDbContext context)
    {
        _context = context;
    }

    public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
    {
        var nic = HttpContext.Session.GetString("PatientNIC");

        if (!string.IsNullOrEmpty(nic))
        {
            ViewBag.UnreadCount = _context.Notifications
                .Count(n => n.PatientNIC == nic && !n.IsRead);
        }

        base.OnActionExecuting(context);
    }
}