using SimpleTimeTracker.Interfaces;
using SimpleTimeTracker.Models;
using System.Collections.Generic;
using System.Text;

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
            if (string.IsNullOrWhiteSpace(userName)) { throw new ArgumentException("Invalid UserName"); }
            if (string.IsNullOrWhiteSpace(project)) { throw new ArgumentException("Invalid Project"); }
            if (string.IsNullOrWhiteSpace(description)) { throw new ArgumentException("Description"); }
            
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
            var entries = _repository.GetAllEntries()  ?? Enumerable.Empty<TimesheetEntry>();

            var totalHoursPerDayPerUser = entries.GroupBy(entry => new { entry.UserName, entry.Date })
                                                 .ToDictionary(
                                                     group => group.Key,
                                                     group => group.Sum(entry => entry.HoursWorked)
                                                 );
                
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("User Name,Date,Project,Description of Tasks,Hours Worked,Total Hours for the Day");
            foreach (var entry in entries)
            {
                var totalHoursForDay = totalHoursPerDayPerUser[new { entry.UserName, entry.Date }];
                csvBuilder.AppendLine(
                    $"{EscapeUnsafeCsvCharacters(entry.UserName)}," +
                    $"{EscapeUnsafeCsvCharacters(entry.Date.ToString())}," +
                    $"{EscapeUnsafeCsvCharacters(entry.Project)}," +
                    $"{EscapeUnsafeCsvCharacters(entry.Description)}," +
                    $"{entry.HoursWorked}," +
                    $"{totalHoursForDay}"
                );
            }

            return csvBuilder.ToString();
        }
        private string EscapeUnsafeCsvCharacters(string unsafeString)
        {
            if (string.IsNullOrWhiteSpace(unsafeString)) return string.Empty;

            return
                (unsafeString.Contains(",") || unsafeString.Contains("\"") || unsafeString.Contains("\n"))
                ? $"\"{unsafeString.Replace("\"", "\"\"")}\"" // Double any quote marks in the string, and then wrap the entire string in quotemarks
                : unsafeString;                               // return the string as-is if no unsafe characters are present             
        }
    }
}
