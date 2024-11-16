using SimpleTimeTracker.Tests.Helpers;
using SimpleTimeTracker.Repositories;

namespace SimpleTimeTracker.Tests.Repositories
{
    public class InMemoryTimesheetRepositoryTests
    {
        [Fact]
        public void NewRepository_Begins_Empty()
        {
            var repository = new InMemoryTimesheetRepository();
            Assert.Equal(0, repository.Entries.Count());
        }
        
        [Fact]
        public void AddEntry_ShouldAddEntry_ToRepository()
        {
            var repository = new InMemoryTimesheetRepository();
            var entry = TestSetupHelper.GenerateEntry();

            repository.AddEntry(entry);
            Assert.Contains(entry, repository.Entries);
        }

        [Fact]
        public void AddEntry_ShouldAddEntry_SingleTimeOnly()
        {
            var repository = new InMemoryTimesheetRepository();
            var entriesStartingCount = repository.Entries.Count();

            repository.AddEntry(TestSetupHelper.GenerateEntry());
            Assert.Equal(entriesStartingCount + 1, repository.Entries.Count());

            repository.AddEntry(TestSetupHelper.GenerateEntry());
            Assert.Equal(entriesStartingCount + 2, repository.Entries.Count());
        }

        [Fact]
        public void AddEntry_ShouldPreserve_DataExactly()
        {
            var user = "MyTestUser";
            var date = DateOnly.FromDateTime(DateTime.Now);
            var project = "MyTestProject";
            var description = "MyTestDescription";
            var hours = 10;
            var repository = new InMemoryTimesheetRepository();

            repository.AddEntry(TestSetupHelper.GenerateEntry(user, date, project, description, hours));
            var addedEntry = repository.Entries.Single();

            Assert.Equal(user, addedEntry.UserName);
            Assert.Equal(date, addedEntry.Date);
            Assert.Equal(project, addedEntry.Project);
            Assert.Equal(description, addedEntry.Description);
            Assert.Equal(hours, addedEntry.HoursWorked);
        }
    }
}