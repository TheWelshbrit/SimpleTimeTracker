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
            return Entries;
        }

        public void AddEntry(TimesheetEntry entry) 
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry), "Provided Timesheet Entry cannot be null");

            Entries.Add(entry);
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
