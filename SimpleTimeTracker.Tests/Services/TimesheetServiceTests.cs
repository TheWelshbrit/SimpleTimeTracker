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
        #endregion
    }
}
