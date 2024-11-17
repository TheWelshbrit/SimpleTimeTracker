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
            if (string.IsNullOrEmpty(userName)) { throw new ArgumentException("Invalid UserName"); }
            if (string.IsNullOrEmpty(project)) { throw new ArgumentException("Invalid Project"); }
            if (string.IsNullOrEmpty(description)) { throw new ArgumentException("Description"); }
            
            if (hoursWorked <= 0) { throw new ArgumentException("Hours Worked must be greater than zero"); }
            if (hoursWorked > 24) { throw new ArgumentException("Hours Worked cannot exceed a full day"); }
            
            var nowDateTime = DateTime.Now;
            var maxDate = DateOnly.FromDateTime(nowDateTime);
            var minDate = DateOnly.FromDateTime(nowDateTime.AddMonths(-2));            
            if (date > maxDate) { throw new ArgumentException("Date cannot be in future."); }
            if (date < minDate) { throw new ArgumentException("Date cannot be excessively in past."); }
         
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
