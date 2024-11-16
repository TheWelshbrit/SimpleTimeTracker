using SimpleTimeTracker.Models;

namespace SimpleTimeTracker.Tests.Helpers
{
    public static class TestSetupHelper
    {
        private static int entryCounter = 0;

        public static TimesheetEntry GenerateEntry(string? user = null, DateOnly? date = null, string? project = null, string? description = null, int? hours = null)
        {
            entryCounter++;
            return new TimesheetEntry
            {
                UserName = user ?? $"User{entryCounter}",
                Date = date ?? DateOnly.FromDateTime(DateTime.Now),
                Project = project ?? $"Project{entryCounter}",
                Description = description ?? "Test Description",
                HoursWorked = hours ?? 2
            };
        }
    }
}
