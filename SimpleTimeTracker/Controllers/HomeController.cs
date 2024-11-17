using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SimpleTimeTracker.Interfaces;
using SimpleTimeTracker.Models;

namespace SimpleTimeTracker.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ITimesheetService _timesheetService;

    public HomeController(ILogger<HomeController> logger, ITimesheetService timesheetService)
    {
        _logger = logger;
        _timesheetService = timesheetService;
    }
    
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpPost]
    public IActionResult AddEntry(string userName, int year, int month, int day, string project, string description, double hoursWorked)
    {
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult GetCsv()
    {
        return null;
    }
}
