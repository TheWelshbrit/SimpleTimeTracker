namespace SimpleTimeTracker.Models
{
    public class TimesheetEntry
    {
        public string UserName { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public string Project { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double HoursWorked { get; set; }
    }
}