using SimpleTimeTracker.Interfaces;
using SimpleTimeTracker.Models;
using System.Collections.Generic;

namespace SimpleTimeTracker.Repositories
{
    public class InMemoryTimesheetRepository : ITimesheetRepository
    {
        protected internal List<TimesheetEntry> Entries { get; private set; } = new List<TimesheetEntry>();

        public IEnumerable<TimesheetEntry> GetAllEntries()
        {
            return Enumerable.Empty<TimesheetEntry>();
        }

        public void AddEntry(TimesheetEntry entry) 
        {

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
