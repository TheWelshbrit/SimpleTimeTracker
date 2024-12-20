using SimpleTimeTracker.Models;

namespace SimpleTimeTracker.Interfaces
{
    public interface ITimesheetService
    {
        IEnumerable<TimesheetEntry> GetAllEntries();
        void AddEntry(string userName, DateOnly date, string project, string description, double hoursWorked);
        IEnumerable<string> GetDistinctUsers();
        IEnumerable<string> GetDistinctProjects();
        string GenerateCsvOutput();
    }
}