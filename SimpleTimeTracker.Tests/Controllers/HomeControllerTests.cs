using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using SimpleTimeTracker.Interfaces;
using SimpleTimeTracker.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;

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
            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                Mock.Of<ITempDataProvider>()
            );
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

        #region AddEntry
        [Fact]
        public void AddEntry_ShouldRedirect_ToIndex()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            
            var result = _controller.AddEntry("TestUser", today.Year, today.Month, today.Day, "TestProject", "TestDescription", 2.5);
            
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public void AddEntry_ShouldCall_ReleventServiceMethod()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            
            var result = _controller.AddEntry("TestUser", today.Year, today.Month, today.Day, "TestProject", "TestDescription", 2.5);
            
            _mockService.Verify(service => service.AddEntry(
                It.IsAny<string>(),
                It.IsAny<DateOnly>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<double>()
            ), Times.Once);
        }

        [Fact]
        public void AddEntry_ShouldNotCall_UnexpectedServiceMethods()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            
            var result = _controller.AddEntry("TestUser", today.Year, today.Month, today.Day, "TestProject", "TestDescription", 2.5);
            
            _mockService.Verify(service => service.AddEntry(
                It.IsAny<string>(),
                It.IsAny<DateOnly>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<double>()
            ), Times.Once);
            _mockService.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddEntry_ShouldCall_ReleventServiceMethod_WithCorrectData()
        {
            
            
            var expectedUser = "TestUser";
            var expectedToday = DateOnly.FromDateTime(DateTime.Now);
            var expectedProject = "TestProject";
            var expectedDescription = "TestDescription";
            var expectedHoursWorked = 2.5;

            var result = _controller.AddEntry(
                expectedUser,
                expectedToday.Year,
                expectedToday.Month,
                expectedToday.Day,
                expectedProject,
                expectedDescription,
                expectedHoursWorked
            );
            
            _mockService.Verify(service => service.AddEntry(
                It.Is<string>(user => user == expectedUser),
                It.Is<DateOnly>(date => date == expectedToday),
                It.Is<string>(project => project == expectedProject),
                It.Is<string>(description => description == expectedDescription),
                It.Is<double>(hours => hours == expectedHoursWorked)
            ), Times.Once);
        }

        [Fact]
        public void AddEntry_ShouldRedirect_ToIndex_WithError_ForInvalidDay()
        {
            var result = _controller.AddEntry("TestUser", 2024, 5, 3000, "TestProject", "TestDescription", 2.5);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.True(_controller.TempData.ContainsKey("Error"));
            Assert.Equal("Invalid date provided.", _controller.TempData["Error"]);
        }

        [Fact]
        public void AddEntry_ShouldRedirect_ToIndex_WithError_ForInvalidMonth()
        {
            var result = _controller.AddEntry("TestUser", 2024, 15, 21, "TestProject", "TestDescription", 2.5);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.True(_controller.TempData.ContainsKey("Error"));
            Assert.Equal("Invalid date provided.", _controller.TempData["Error"]);
        }

        [Fact]
        public void AddEntry_ShouldRedirect_ToIndex_WithError_ForInvalidYear()
        {
            var result = _controller.AddEntry("TestUser", -1066, 5, 21, "TestProject", "TestDescription", 2.5);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.True(_controller.TempData.ContainsKey("Error"));
            Assert.Equal("Invalid date provided.", _controller.TempData["Error"]);
        }

        [Fact]
        public void AddEntry_ShouldNotCallService_OnInvalidDateInput()
        {
            var result1 = _controller.AddEntry("TestUser", 2024, 5, 3000, "TestProject", "TestDescription", 2.5);
            var result2 = _controller.AddEntry("TestUser", 2024, 15, 21, "TestProject", "TestDescription", 2.5);
            var result3 = _controller.AddEntry("TestUser", -1066, 5, 21, "TestProject", "TestDescription", 2.5);

            _mockService.Verify(service => service.AddEntry(
                It.IsAny<string>(),
                It.IsAny<DateOnly>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<double>()
            ), Times.Never);
            _mockService.VerifyNoOtherCalls();
        }

        [Fact]
        public void AddEntry_ShouldRedirect_ToIndex_WithErrorMessage_WhenServiceThrowsArgumentException()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            _mockService
                .Setup(service => service.AddEntry(It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()))
                .Throws(new ArgumentException("Invalid data provided"));

            var result = _controller.AddEntry("TestUser", today.Year, today.Month, today.Day, "TestProject", "TestDescription", 2.5);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.True(_controller.TempData.ContainsKey("Error"));
            Assert.Equal("Invalid data provided", _controller.TempData["Error"]);
        }

        [Fact]
        public void AddEntry_ShouldRedirect_ToIndex_WithErrorMessage_WhenServiceThrowsUnexpectedException()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            _mockService
                .Setup(service => service.AddEntry(It.IsAny<string>(), It.IsAny<DateOnly>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()))
                .Throws(new Exception("Invalid data provided"));

            var result = _controller.AddEntry("TestUser", today.Year, today.Month, today.Day, "TestProject", "TestDescription", 2.5);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.True(_controller.TempData.ContainsKey("Error"));
            Assert.Equal("An unexpected error occurred. Please try again later.", _controller.TempData["Error"]);
        }
        #endregion

        #region GetCsv
        [Fact]
        public void GetCsv_ShouldCallServiceGenerateCsvOutput()
        {
            _mockService.Setup(service => service.GenerateCsvOutput()).Returns(string.Empty);
            
            var result = _controller.GetCsv();
            
            _mockService.Verify(service => service.GenerateCsvOutput(), Times.Once);
        }

        [Fact]
        public void GetCsv_ShouldReturnFileResult_WithCorrectContentTypeAndName()
        {
            _mockService.Setup(service => service.GenerateCsvOutput()).Returns("User Name,Date,Project,...");
            
            var result = _controller.GetCsv() as FileContentResult;
            
            Assert.NotNull(result);
            Assert.Equal("text/csv", result.ContentType);
            Assert.Equal("timesheet.csv", result.FileDownloadName);
        }

        [Fact]
        public void GetCsv_ShouldReturnFileResult_WithCorrectContent()
        {
            var csvContent = "User Name,Date,Project,...";
            _mockService.Setup(service => service.GenerateCsvOutput()).Returns(csvContent);

            var result = _controller.GetCsv() as FileContentResult;

            Assert.NotNull(result);
            var content = System.Text.Encoding.UTF8.GetString(result.FileContents);
            Assert.Equal(csvContent, content);
        }

        [Fact]
        public void GetCsv_ShouldHandleServiceException_Gracefully()
        {
            _mockService.Setup(service => service.GenerateCsvOutput()).Throws(new Exception("Service failed"));

            var result = _controller.GetCsv() as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.True(_controller.TempData.ContainsKey("Error"));
            Assert.Equal("An unexpected error occurred. Please try again later.", _controller.TempData["Error"]);
        }
        #endregion
        #endregion
    }
}