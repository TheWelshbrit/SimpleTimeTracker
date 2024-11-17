using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using SimpleTimeTracker.Interfaces;
using SimpleTimeTracker.Controllers;
using Microsoft.Extensions.Logging;

namespace SimpleTimeTracker.Tests.Controllers
{
    public class HomeControllerTests
    {
        #region Setup
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly Mock<ITimesheetService> _mockService;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _mockService = new Mock<ITimesheetService>();
            _controller = new HomeController(_mockLogger.Object, _mockService.Object);
        }
        #endregion
        
        #region Tests
        [Fact]
        public void Index_ShouldReturn_IndexView()
        {
            var result = _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult?.ViewName ?? "Index");
        }
        #endregion
    }
}