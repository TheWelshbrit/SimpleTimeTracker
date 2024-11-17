using SimpleTimeTracker.Interfaces;
using SimpleTimeTracker.Models;
using System.Collections.Generic;

namespace SimpleTimeTracker.Services
{
    public class TimesheetService : ITimesheetService
    {
        private readonly ITimesheetRepository _repository;

        public TimesheetService(ITimesheetRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<TimesheetEntry> GetAllEntries()
        {
            return Enumerable.Empty<TimesheetEntry>();
        }

        public void AddEntry(string userName, DateOnly date, string project, string description, double hoursWorked)
        {
            _repository.AddEntry(new TimesheetEntry{
                UserName = userName,
                Date = date,
                Project = project,
                Description = description,
                HoursWorked = hoursWorked
            });
        }

        public IEnumerable<string> GetDistinctProjects()
        {
            return Enumerable.Empty<string>();
        }

        public IEnumerable<string> GetDistinctUsers()
        {
            return Enumerable.Empty<string>();
        }

        public string GenerateCsvOutput()
        {
            return string.Empty;
        }
    }
}
