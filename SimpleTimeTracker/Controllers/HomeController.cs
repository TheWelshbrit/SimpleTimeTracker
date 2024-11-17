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
        DateOnly givenDate;

        try 
        {
            givenDate = new DateOnly(year, month, day);
        }
        catch (ArgumentOutOfRangeException)
        {
            TempData["Error"] = "Invalid date provided.";
            return RedirectToAction("Index");
        }

        _timesheetService.AddEntry(
            userName,
            givenDate,
            project,
            description,
            hoursWorked
        );

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult GetCsv()
    {
        return null;
    }
}
