using Xunit;
using CalcAPI.Web.Controllers;
using CalcAPI.Domain.Entities;
using CalcAPI.Application.Services;
using Moq;
using Microsoft.AspNetCore.Mvc;

namespace CalcAPI.Tests
{
    public class CalculatorTests
    {
        private readonly Mock<ILogService> _mockLogService;
        private readonly AddController _addController;
        private readonly DivideController _divideController;

        public CalculatorTests()
        {
            _mockLogService = new Mock<ILogService>();
            _addController = new AddController(_mockLogService.Object);
            _divideController = new DivideController(_mockLogService.Object);
        }

        [Fact]
        public async Task Add_ValidNumbers_ReturnsCorrectSum()
        {
            // Arrange
            var request = new Request
            {
                Value1 = 5,
                Value2 = 3,
                User = "testUser"
            };

            // Act
            var result = await _addController.Add(request);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<Response>(actionResult.Value);
            Assert.Equal(8, response.Result);
        }

        [Fact]
        public async Task Divide_ByZero_ReturnsBadRequest()
        {
            // Arrange
            var request = new Request
            {
                Value1 = 10,
                Value2 = 0,
                User = "testUser"
            };

            // Act
            var result = await _divideController.Divide(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Add_NullValues_ReturnsBadRequest()
        {
            // Arrange
            var request = new Request
            {
                Value1 = null,
                Value2 = null,
                User = "testUser"
            };

            // Act
            var result = await _addController.Add(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}