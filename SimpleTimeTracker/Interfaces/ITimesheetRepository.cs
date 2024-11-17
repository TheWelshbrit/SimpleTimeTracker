using SimpleTimeTracker.Models;

namespace SimpleTimeTracker.Interfaces
{
    public interface ITimesheetRepository
    {
        IEnumerable<TimesheetEntry> GetAllEntries();
        void AddEntry(TimesheetEntry entry);
        IEnumerable<string> GetDistinctUsers();
        IEnumerable<string> GetDistinctProjects();
    }
}