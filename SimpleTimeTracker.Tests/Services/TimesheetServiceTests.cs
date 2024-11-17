using Moq;
using SimpleTimeTracker.Interfaces;
using SimpleTimeTracker.Models;
using SimpleTimeTracker.Services;

namespace SimpleTimeTracker.Tests.Services
{
    public class TimesheetServiceTests
    {
        private readonly Mock<ITimesheetRepository> _mockRepository;
        private readonly TimesheetService _service;

        public TimesheetServiceTests()
        {
            _mockRepository = new Mock<ITimesheetRepository>();
            _service = new TimesheetService(_mockRepository.Object);
        }
    }
}
