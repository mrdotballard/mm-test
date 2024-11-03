using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CalcAPI.Application.Services;
using CalcAPI.Domain.Entities;
using CalcAPI.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using Xunit;

namespace CalcAPI.Tests
{
    public class CalculatorApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        readonly WebApplicationFactory<Program> _factory;
        readonly HttpClient _client;

        public CalculatorApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _client.DefaultRequestHeaders.Add("ApiKey", "your-test-api-key");
        }

        [Fact]
        public async Task Add_ValidInput_ReturnsCorrectResult()
        {
            // Arrange
            var request = new Request
            {
                Value1 = 10,
                Value2 = 5,
                User = "testUser"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/add", request);
            var result = await response.Content.ReadFromJsonAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.Equal(15, result.Result);
        }

        [Fact]
        public async Task Add_MissingApiKey_ReturnsUnauthorized()
        {
            // Arrange
            var clientWithoutAuth = _factory.CreateClient();
            var request = new Request
            {
                Value1 = 10,
                Value2 = 5,
                User = "testUser"
            };

            // Act
            var response = await clientWithoutAuth.PostAsJsonAsync("/api/add", request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Divide_ValidInput_ReturnsCorrectResult()
        {
            // Arrange
            var request = new Request
            {
                Value1 = 10,
                Value2 = 2,
                User = "testUser"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/divide", request);
            var result = await response.Content.ReadFromJsonAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.Equal(5, result.Result);
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
            var response = await _client.PostAsJsonAsync("/api/divide", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Multiply_ValidInput_ReturnsCorrectResult()
        {
            // Arrange
            var request = new Request
            {
                Value1 = 10,
                Value2 = 5,
                User = "testUser"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/multiply", request);
            var result = await response.Content.ReadFromJsonAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.Equal(50, result.Result);
        }

        [Fact]
        public async Task Subtract_ValidInput_ReturnsCorrectResult()
        {
            // Arrange
            var request = new Request
            {
                Value1 = 10,
                Value2 = 5,
                User = "testUser"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/subtract", request);
            var result = await response.Content.ReadFromJsonAsync<Response>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.Equal(5, result.Result);
        }

        [Theory]
        [InlineData(null, 5)]
        [InlineData(5, null)]
        [InlineData(null, null)]
        public async Task AllOperations_NullValues_ReturnBadRequest(decimal? value1, decimal? value2)
        {
            // Arrange
            var request = new Request
            {
                Value1 = value1,
                Value2 = value2,
                User = "testUser"
            };

            // Act & Assert
            var addResponse = await _client.PostAsJsonAsync("/api/add", request);
            var subtractResponse = await _client.PostAsJsonAsync("/api/subtract", request);
            var multiplyResponse = await _client.PostAsJsonAsync("/api/multiply", request);
            var divideResponse = await _client.PostAsJsonAsync("/api/divide", request);

            Assert.Equal(HttpStatusCode.BadRequest, addResponse.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, subtractResponse.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, multiplyResponse.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, divideResponse.StatusCode);
        }

        [Fact]
        public async Task GetLogs_ValidUser_ReturnsResults()
        {
            // Arrange
            string user = "testUser";

            // Act
            var response = await _client.GetAsync($"/api/add?user={user}");
            var logs = await response.Content.ReadFromJsonAsync<List<LogRequest>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(logs);
        }

        [Fact]
        public async Task GetLogs_EmptyUser_ReturnsBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/add?user=");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }

    public class LogServiceTests
    {
        private readonly Mock<ILogRepository> _mockRepository;
        private readonly ILogService _logService;

        public LogServiceTests()
        {
            _mockRepository = new Mock<ILogRepository>();
            _logService = new LogService(_mockRepository.Object);
        }

        [Fact]
        public async Task LogRequestAsync_ValidInput_CallsRepository()
        {
            // Arrange
            string user = "testUser";
            string operation = "Add";
            decimal value1 = 10;
            decimal value2 = 5;
            decimal result = 15;

            // Act
            await _logService.LogRequestAsync(user, operation, value1, value2, result);

            // Assert
            _mockRepository.Verify(r => r.AddAsync(
                It.Is<LogRequest>(log =>
                    log.User == user &&
                    log.Operation == operation &&
                    log.InputValue1 == value1 &&
                    log.InputValue2 == value2 &&
                    log.Result == result
                )), Times.Once);
        }

        [Fact]
        public async Task GetLogsAsync_ValidInput_ReturnsLogs()
        {
            // Arrange
            string user = "testUser";
            string operation = "Add";
            var expectedLogs = new List<LogRequest>
            {
                new LogRequest { User = user, Operation = operation }
            };
            _mockRepository.Setup(r => r.GetAsync(user, operation))
                .ReturnsAsync(expectedLogs);

            // Act
            var result = await _logService.GetLogsAsync(user, operation);

            // Assert
            Assert.Equal(expectedLogs, result);
            _mockRepository.Verify(r => r.GetAsync(user, operation), Times.Once);
        }
    }
}