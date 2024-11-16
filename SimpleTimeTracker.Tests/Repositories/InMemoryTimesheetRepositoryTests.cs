using SimpleTimeTracker.Tests.Helpers;
using SimpleTimeTracker.Repositories;

namespace SimpleTimeTracker.Tests.Repositories
{
    public class InMemoryTimesheetRepositoryTests
    {
        #region AddEntry
        [Fact]
        public void NewRepository_Begins_Empty()
        {
            var repository = new InMemoryTimesheetRepository();
            Assert.Empty(repository.Entries);
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

        [Fact]
        public void AddEntry_ShouldThrowException_WhenEntryIsNull()
        {
            var repository = new InMemoryTimesheetRepository();

            Assert.Throws<ArgumentNullException>(() => repository.AddEntry(null!)); // "!" suppresses compiler warning for passing null to non-nullable input
        }

        [Fact]
        public void AddEntry_ShouldAllow_DuplicateEntries()
        {
            var repository = new InMemoryTimesheetRepository();
            var entry = TestSetupHelper.GenerateEntry();

            repository.AddEntry(entry);
            repository.AddEntry(entry);

            Assert.Equal(2, repository.Entries.Count());
            Assert.Contains(entry, repository.Entries);
        }

        [Fact]
        public void AddEntry_ShouldHandle_MultipleEntries()
        {
            var repository = new InMemoryTimesheetRepository();
            var entry1 = TestSetupHelper.GenerateEntry();
            var entry2 = TestSetupHelper.GenerateEntry();
            var entry3 = TestSetupHelper.GenerateEntry();

            repository.AddEntry(entry1);
            repository.AddEntry(entry2);
            repository.AddEntry(entry3);

            Assert.Equal(3, repository.Entries.Count());
            Assert.Contains(entry1, repository.Entries);
            Assert.Contains(entry2, repository.Entries);
            Assert.Contains(entry3, repository.Entries);
        }

        [Fact]
        public void AddEntry_ShouldStoreBlankRecord_IfProvidedBlankData()
        {
            var repository = new InMemoryTimesheetRepository();

            var blankEntry = TestSetupHelper.GenerateEntry(string.Empty, default(DateOnly), string.Empty, string.Empty, 0);
            repository.AddEntry(blankEntry);

            Assert.Single(repository.Entries);
            Assert.Contains(blankEntry, repository.Entries);
        }
        #endregion

        #region GetAllEntries
        [Fact]
        public void GetAllEntries_ShouldReturnEmpty_WhenRepositoryIsNew()
        {
            var repository = new InMemoryTimesheetRepository();

            var recievedEntries = repository.GetAllEntries();

            Assert.Empty(recievedEntries);
        }

        [Fact]
        public void GetAllEntries_ShouldReturnAllEntries_AndReflectRepositoryState()
        {
            var repository = new InMemoryTimesheetRepository();
            var entry1 = TestSetupHelper.GenerateEntry();
            var entry2 = TestSetupHelper.GenerateEntry();
            var entry3 = TestSetupHelper.GenerateEntry();
            var entry4 = TestSetupHelper.GenerateEntry();

            // add and verify single entry
            repository.AddEntry(entry1);

            var recievedEntries = repository.GetAllEntries();
            Assert.Single(recievedEntries);
            Assert.Contains(entry1, recievedEntries);


            // add additional entries and verify updated state
            repository.AddEntry(entry2);
            repository.AddEntry(entry3);
            repository.AddEntry(entry4);
            recievedEntries = repository.GetAllEntries();

            Assert.Equal(4, recievedEntries.Count());
            Assert.Contains(entry1, recievedEntries);
            Assert.Contains(entry2, recievedEntries);
            Assert.Contains(entry3, recievedEntries);
            Assert.Contains(entry4, recievedEntries);
        }
        #endregion
    }
}