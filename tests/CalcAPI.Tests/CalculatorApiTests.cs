using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using CalcAPI.Web.Controllers;
using CalcAPI.Domain.Entities;
using CalcAPI.Application.Services;

namespace CalcAPI.Tests
{
    public class CalculatorControllerTests
    {
        private readonly Mock<ILogService> _mockLogService;
        private readonly AddController _addController;
        private readonly DivideController _divideController;
        private readonly MultiplyController _multiplyController;
        private readonly SubtractController _subtractController;

        public CalculatorControllerTests()
        {
            _mockLogService = new Mock<ILogService>();
            _addController = new AddController(_mockLogService.Object);
            _divideController = new DivideController(_mockLogService.Object);
            _multiplyController = new MultiplyController(_mockLogService.Object);
            _subtractController = new SubtractController(_mockLogService.Object);
        }

        public static IEnumerable<object[]> NullTestData()
        {
            yield return new object[] { null, (decimal?)5 };
            yield return new object[] { (decimal?)5, null };
            yield return new object[] { null, null };
        }

        [Theory]
        [MemberData(nameof(NullTestData))]
        public async Task Add_NullValues_ReturnsBadRequest(decimal? value1, decimal? value2)
        {
            // Arrange
            var request = new Request
            {
                Value1 = value1,
                Value2 = value2,
                User = "testUser"
            };

            // Act
            var result = await _addController.Add(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }


        [Fact]
        public async Task Add_ValidInput_ReturnsCorrectResult()
        {
            // Arrange
            var request = new Request
            {
                Value1 = 5.0m, // Using decimal literal
                Value2 = 3.0m,
                User = "testUser"
            };

            // Act
            var result = await _addController.Add(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<Response>(okResult.Value);
            Assert.Equal(8.0m, response.Result);
        }

        [Fact]
        public async Task Divide_ByZero_ReturnsBadRequest()
        {
            // Arrange
            var request = new Request
            {
                Value1 = 10.0m,
                Value2 = 0.0m,
                User = "testUser"
            };

            // Act
            var result = await _divideController.Divide(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task Multiply_ValidInput_ReturnsCorrectResult()
        {
            // Arrange
            var request = new Request
            {
                Value1 = 4.0m,
                Value2 = 5.0m,
                User = "testUser"
            };

            // Act
            var result = await _multiplyController.Multiply(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<Response>(okResult.Value);
            Assert.Equal(20.0m, response.Result);
        }

        [Fact]
        public async Task Subtract_ValidInput_ReturnsCorrectResult()
        {
            // Arrange
            var request = new Request
            {
                Value1 = 10.0m,
                Value2 = 3.0m,
                User = "testUser"
            };

            // Act
            var result = await _subtractController.Subtract(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<Response>(okResult.Value);
            Assert.Equal(7.0m, response.Result);
        }
    }
}