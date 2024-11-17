using System.Net.Mail;
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
    
        #region GenerateCsvOutput
        [Fact]
        public void GenerateCsvOutput_ShouldRetrieveAllEntries_FromRepository()
        {
            _service.GenerateCsvOutput();
            _mockRepository.Verify(repo => repo.GetAllEntries(), Times.Once);
        }

        [Fact]
        public void GenerateCsvOutput_ShouldReturnFormattedCsv()
        {
            _mockRepository.Setup(repo => repo.GetAllEntries()).Returns(new List<TimesheetEntry>());
            var expectedHeader = "User Name,Date,Project,Description of Tasks,Hours Worked,Total Hours for the Day";

            var csvData = _service.GenerateCsvOutput();
            var csvLines = csvData.Split(Environment.NewLine);

            Assert.Equal(expectedHeader, csvLines[0]);
        }

        [Fact]
        public void GenerateCsvOutput_ShouldReturnFormattedCsv_WithCorrectDataRows()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var entries = new List<TimesheetEntry>
            {
                new TimesheetEntry { UserName = "Alice", Date = today, Project = "Project1", Description = "Task 1", HoursWorked = 1 },
                new TimesheetEntry { UserName = "Bob", Date = today, Project = "Project2", Description = "Task 2", HoursWorked = 2 },
                new TimesheetEntry { UserName = "Cathy", Date = today, Project = "Project3", Description = "Task 3", HoursWorked = 3 },
                new TimesheetEntry { UserName = "Dave", Date = today, Project = "Project4", Description = "Task 4", HoursWorked = 4 },
                new TimesheetEntry { UserName = "Ellen", Date = today, Project = "Project5", Description = "Task 5", HoursWorked = 5 },
                new TimesheetEntry { UserName = "Farquad", Date = today, Project = "Project6", Description = "Task 6", HoursWorked = 6 }
            };
            _mockRepository.Setup(repo => repo.GetAllEntries()).Returns(entries);
            
            var expectedHeader = "User Name,Date,Project,Description of Tasks,Hours Worked,Total Hours for the Day";
            var expectedRow1 = $"{entries[0].UserName},{entries[0].Date},{entries[0].Project},{entries[0].Description},{entries[0].HoursWorked},{entries[0].HoursWorked}";
            var expectedRow2 = $"{entries[1].UserName},{entries[1].Date},{entries[1].Project},{entries[1].Description},{entries[1].HoursWorked},{entries[1].HoursWorked}";
            var expectedRow3 = $"{entries[2].UserName},{entries[2].Date},{entries[2].Project},{entries[2].Description},{entries[2].HoursWorked},{entries[2].HoursWorked}";
            var expectedRow4 = $"{entries[3].UserName},{entries[3].Date},{entries[3].Project},{entries[3].Description},{entries[3].HoursWorked},{entries[3].HoursWorked}";
            var expectedRow5 = $"{entries[4].UserName},{entries[4].Date},{entries[4].Project},{entries[4].Description},{entries[4].HoursWorked},{entries[4].HoursWorked}";
            var expectedRow6 = $"{entries[5].UserName},{entries[5].Date},{entries[5].Project},{entries[5].Description},{entries[5].HoursWorked},{entries[5].HoursWorked}";
            

            var csvData = _service.GenerateCsvOutput();
            var csvLines = csvData.Split(Environment.NewLine);

            Assert.Equal(expectedHeader, csvLines[0]);
            Assert.Equal(expectedRow1, csvLines[1]);
            Assert.Equal(expectedRow2, csvLines[2]);
            Assert.Equal(expectedRow3, csvLines[3]);
            Assert.Equal(expectedRow4, csvLines[4]);
            Assert.Equal(expectedRow5, csvLines[5]);
            Assert.Equal(expectedRow6, csvLines[6]);
        }

        [Fact]
        public void GenerateCsvOutput_ShouldReturnFormattedCsv_WithCalculatedTotalHoursForDay()
        {
            var nowDateTime = DateTime.Now;
            var today = DateOnly.FromDateTime(nowDateTime);
            var yesterday = DateOnly.FromDateTime(nowDateTime.AddDays(-1));

            var entries = new List<TimesheetEntry>
            {
                new TimesheetEntry { UserName = "Bob", Date = yesterday, Project = "Project1", Description = "Task 1A", HoursWorked = 3 },
                new TimesheetEntry { UserName = "Bob", Date = yesterday, Project = "Project2", Description = "Task 2A", HoursWorked = 2 },
                new TimesheetEntry { UserName = "Alice", Date = yesterday, Project = "Project1", Description = "Task 1B", HoursWorked = 2 },
                new TimesheetEntry { UserName = "Bob", Date = today, Project = "Project2", Description = "Task 2B", HoursWorked = 3 },
                new TimesheetEntry { UserName = "Alice", Date = today, Project = "Project1", Description = "Task 1C", HoursWorked = 5 },
                new TimesheetEntry { UserName = "Alice", Date = today, Project = "Project2", Description = "Task 2C", HoursWorked = 4 }
            };
            _mockRepository.Setup(repo => repo.GetAllEntries()).Returns(entries);
            
            var expectedTotalHours_Alice_Yesterday = entries.Where(timeSheet => timeSheet.Date == yesterday && timeSheet.UserName == "Alice").Sum(timeSheet => timeSheet.HoursWorked);
            var expectedTotalHours_Alice_Today = entries.Where(timeSheet => timeSheet.Date == today && timeSheet.UserName == "Alice").Sum(timeSheet => timeSheet.HoursWorked);
            var expectedTotalHours_Bob_Yesterday = entries.Where(timeSheet => timeSheet.Date == yesterday && timeSheet.UserName == "Bob").Sum(timeSheet => timeSheet.HoursWorked);
            var expectedTotalHours_Bob_Today = entries.Where(timeSheet => timeSheet.Date == today && timeSheet.UserName == "Bob").Sum(timeSheet => timeSheet.HoursWorked);
            
            var expectedHeader = "User Name,Date,Project,Description of Tasks,Hours Worked,Total Hours for the Day";
            var expectedRow1 = $"{entries[0].UserName},{entries[0].Date},{entries[0].Project},{entries[0].Description},{entries[0].HoursWorked},{expectedTotalHours_Bob_Yesterday}";
            var expectedRow2 = $"{entries[1].UserName},{entries[1].Date},{entries[1].Project},{entries[1].Description},{entries[1].HoursWorked},{expectedTotalHours_Bob_Yesterday}";
            var expectedRow3 = $"{entries[2].UserName},{entries[2].Date},{entries[2].Project},{entries[2].Description},{entries[2].HoursWorked},{expectedTotalHours_Alice_Yesterday}";
            var expectedRow4 = $"{entries[3].UserName},{entries[3].Date},{entries[3].Project},{entries[3].Description},{entries[3].HoursWorked},{expectedTotalHours_Bob_Today}";
            var expectedRow5 = $"{entries[4].UserName},{entries[4].Date},{entries[4].Project},{entries[4].Description},{entries[4].HoursWorked},{expectedTotalHours_Alice_Today}";
            var expectedRow6 = $"{entries[5].UserName},{entries[5].Date},{entries[5].Project},{entries[5].Description},{entries[5].HoursWorked},{expectedTotalHours_Alice_Today}";
            

            var csvData = _service.GenerateCsvOutput();
            var csvLines = csvData.Split(Environment.NewLine);

            Assert.Equal(expectedHeader, csvLines[0]);
            Assert.Equal(expectedRow1, csvLines[1]);
            Assert.Equal(expectedRow2, csvLines[2]);
            Assert.Equal(expectedRow3, csvLines[3]);
            Assert.Equal(expectedRow4, csvLines[4]);
            Assert.Equal(expectedRow5, csvLines[5]);
            Assert.Equal(expectedRow6, csvLines[6]);
        }

        [Fact]
        public void GenerateCsvOutput_ShouldHandleNullReturn_FromRepository()
        {
            _mockRepository.Setup(repo => repo.GetAllEntries()).Returns((IEnumerable<TimesheetEntry>)null!);

            var csv = _service.GenerateCsvOutput();

            Assert.Equal($"User Name,Date,Project,Description of Tasks,Hours Worked,Total Hours for the Day{Environment.NewLine}", csv);
        }

        [Fact]
        public void GenerateCsvOutput_ShouldEscapeSpecialCharacters()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var entries = new List<TimesheetEntry>
            {
                new TimesheetEntry { UserName = "User", Date = today, Project = "Project,1", Description = "Task\n\"1\"", HoursWorked = 5 }
            };
            _mockRepository.Setup(repo => repo.GetAllEntries()).Returns(entries);
            var expectedHeader = "User Name,Date,Project,Description of Tasks,Hours Worked,Total Hours for the Day";
            var expectedRow = $"User,{today},\"Project,1\",\"Task\n\"\"1\"\"\",5,5";

            var csv = _service.GenerateCsvOutput();
            var csvLines = csv.Split(Environment.NewLine);

            Assert.Equal(expectedHeader, csvLines[0]);
            Assert.Equal(expectedRow, csvLines[1]);
        }
        #endregion
    }
}
