using Moq;
using SimpleTimeTracker.Interfaces;
using SimpleTimeTracker.Models;
using SimpleTimeTracker.Services;

namespace SimpleTimeTracker.Tests.Services
{
    public class TimesheetServiceTests
    {
        #region Setup
        private readonly Mock<ITimesheetRepository> _mockRepository;
        private readonly TimesheetService _service;

        public TimesheetServiceTests()
        {
            _mockRepository = new Mock<ITimesheetRepository>();
            _service = new TimesheetService(_mockRepository.Object);
        }
        #endregion

        #region AddEntry
        [Fact]
        public void AddEntry_ShouldCallRepository_AddEntry_Once()
        {
            _service.AddEntry("TestUser", DateOnly.FromDateTime(DateTime.Now), "TestProject", "TestDescription", 5);

            _mockRepository.Verify(repo => 
                repo.AddEntry(It.IsAny<TimesheetEntry>()),
                Times.Once
            );
        }

        [Fact]
        public void AddEntry_ShouldCallRepository_WithValidData()
        {
            var user = "TestUser";
            var date = DateOnly.FromDateTime(DateTime.Now);
            var project = "TestProject";
            var description = "TestDescription";
            var hoursWorked = 5;

            _service.AddEntry(user, date, project, description, hoursWorked);

            _mockRepository.Verify(repo => 
                repo.AddEntry(It.Is<TimesheetEntry>(entry =>
                    entry.UserName == user &&
                    entry.Date == date &&
                    entry.Project == project &&
                    entry.Description == description &&
                    entry.HoursWorked == hoursWorked
                )),
                Times.Once
            );
        }

        [Fact]
        public void AddEntry_ShouldThrowException_WhenInputsAreNullOrEmpty()
        {
            var date = DateOnly.FromDateTime(DateTime.Now);

            Assert.Throws<ArgumentException>(() => _service.AddEntry(null, date, "Project", "Description", 1));         // Null user
            Assert.Throws<ArgumentException>(() => _service.AddEntry(string.Empty, date, "Project", "Description", 1)); // Empty user
            Assert.Throws<ArgumentException>(() => _service.AddEntry("User", date, null, "Description", 1));            // Null project
            Assert.Throws<ArgumentException>(() => _service.AddEntry("User", date, string.Empty, "Description", 1));    // Empty project
            Assert.Throws<ArgumentException>(() => _service.AddEntry("User", date, "Project", null, 1));                // Null description
            Assert.Throws<ArgumentException>(() => _service.AddEntry("User", date, "Project", string.Empty, 1));        // Empty description
        }

        [Fact]
        public void AddEntry_ShouldThrowException_WhenHoursWorkedIsInvalid()
        {
            var date = DateOnly.FromDateTime(DateTime.Now);

            Assert.Throws<ArgumentException>(() => _service.AddEntry("User", date, "Project", "Description", -0.01)); // negative hours
            Assert.Throws<ArgumentException>(() => _service.AddEntry("User", date, "Project", "Description", 0));     // zero hours
            Assert.Throws<ArgumentException>(() => _service.AddEntry("User", date, "Project", "Description", 24.01)); // more hours than a day allows
        }

        [Fact]
        public void AddEntry_ShouldThrowException_WhenDateIsInFuture_OrExcessivelyInPast()
        {
            var futureDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var pastDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(-2).AddDays(-1));

            Assert.Throws<ArgumentException>(() => _service.AddEntry("User", futureDate, "Project", "Description", 1)); // Date in future
            Assert.Throws<ArgumentException>(() => _service.AddEntry("User", pastDate, "Project", "Description", 1));   // Date excessively in past
        }

        [Fact]
        public void AddEntry_ShouldThrowException_WhenInputsAreWhiteSpace()
        {
            var date = DateOnly.FromDateTime(DateTime.Now);

            Assert.Throws<ArgumentException>(() => _service.AddEntry("   ", date, "Project", "Description", 1));         // Null user
            Assert.Throws<ArgumentException>(() => _service.AddEntry("User", date, "   ", "Description", 1));            // Null project
            Assert.Throws<ArgumentException>(() => _service.AddEntry("User", date, "Project", "   ", 1));                // Null description
        }

        [Fact]
        public void AddEntry_ShouldAllow_MultipleEntriesPerDay_ForAGivenUser()
        {
            var date = DateOnly.FromDateTime(DateTime.Now);
            
            _service.AddEntry("User", date, "Project1", "Description", 12);

            try 
            {
                _service.AddEntry("User", date, "Project2", "Description", 8);
            }
            catch (Exception e)
            {
                Assert.False(true, $"Expected no exception, but got {e.Message} whilst attempting to add second valid entry");
            }
        
            try 
            {
                _service.AddEntry("User", date, "Project3", "Description", 4);
            }
            catch (Exception e)
            {
                Assert.False(true, $"Expected no exception, but got {e.Message} whilst attempting to add final valid entry");
            }
        }

        // TODO: return to advanced edge case should time permit
        // [Fact]
        // public void AddEntry_ShouldThrowException_WhenTotalHoursWorked_ForAUserInADay_ExceedAFullDay()
        // {
        //     var date = DateOnly.FromDateTime(DateTime.Now);
            
        //     _service.AddEntry("User", date, "Project", "Description", 12);

        //     Assert.Throws<ArgumentException>(() => _service.AddEntry("User", date, "Project", "Description", 12.01));
        // }
        #endregion
    }
}
