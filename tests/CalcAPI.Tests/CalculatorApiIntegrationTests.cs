using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using System.Text.Json;
using CalcAPI.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using CalcAPI.Application.Services;
using CalcAPI.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Security.Claims;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace CalcAPI.IntegrationTests
{
    public class CalculatorApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly string _apiKey;
        public CalculatorApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    // Clear existing configurations
                    config.Sources.Clear();

                    // Add test appsettings from test project directory
                    var projectDir = Directory.GetCurrentDirectory();
                    config.AddJsonFile(Path.Combine(projectDir, "appsettings.test.json"));
                });

                builder.ConfigureServices(services =>
                {
                    // Remove the existing log repository
                    var logRepoDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(ILogRepository));
                    if (logRepoDescriptor != null)
                    {
                        services.Remove(logRepoDescriptor);
                    }

                    // Add in-memory test repository
                    services.AddSingleton<ILogRepository, TestLogRepository>();
                });
            });

            _client = _factory.CreateClient();

            // Get API key from test configuration
            var configuration = _factory.Services.GetRequiredService<IConfiguration>();
            _apiKey = configuration["ApiKey"] ??
                throw new InvalidOperationException("API Key not found in test configuration");

            // Add API key to default headers
            _client.DefaultRequestHeaders.Add("ApiKey", _apiKey);
        }

        [Fact]
        public async Task Multiply_WithoutApiKey_ReturnsUnauthorized()
        {
            // Arrange
            var clientWithoutAuth = _factory.CreateClient();  // Create new client without API key
            var request = new Request
            {
                Value1 = 5,
                Value2 = 3,
                User = "testUser"
            };

            var jsonContent = JsonSerializer.Serialize(request);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await clientWithoutAuth.PostAsync("/api/multiply", stringContent);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Multiply_WithInvalidApiKey_ReturnsUnauthorized()
        {
            // Arrange
            var clientWithInvalidAuth = _factory.CreateClient();
            clientWithInvalidAuth.DefaultRequestHeaders.Add("X-API-Key", "invalid-key");

            var request = new Request
            {
                Value1 = 5,
                Value2 = 3,
                User = "testUser"
            };

            var jsonContent = JsonSerializer.Serialize(request);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await clientWithInvalidAuth.PostAsync("/api/multiply", stringContent);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData(5, 3, 15)]
        [InlineData(-5, 5, -25)]
        [InlineData(0, 5, 0)]
        public async Task Multiply_WithValidApiKey_ReturnsCorrectResult(decimal value1, decimal value2, decimal expectedResult)
        {
            // Arrange
            var request = new Request
            {
                Value1 = value1,
                Value2 = value2,
                User = "testUser"
            };

            var jsonContent = JsonSerializer.Serialize(request);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/multiply", stringContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Response>(jsonResponse,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(expectedResult, result.Result);
        }

        [Theory]
        [InlineData(5, 3, 15)]
        [InlineData(-5, 5, -25)]
        [InlineData(0, 5, 0)]
        public async Task Multiply_ReturnsCorrectResult(decimal value1, decimal value2, decimal expectedResult)
        {
            // Arrange
            var request = new Request
            {
                Value1 = value1,
                Value2 = value2,
                User = "testUser"
            };

            var jsonContent = JsonSerializer.Serialize(request);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/multiply", stringContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Response>(jsonResponse,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Equal(expectedResult, result.Result);
        }

        [Fact]
        public async Task Multiply_WithoutAuth_ReturnsUnauthorized()
        {
            // Arrange
            var client = _factory.CreateClient(); // Create client without auth header
            var request = new Request
            {
                Value1 = 5,
                Value2 = 3,
                User = "testUser"
            };

            var jsonContent = JsonSerializer.Serialize(request);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/api/multiply", stringContent);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Get_ReturnsCorrectLogs()
        {
            // Arrange
            string testUser = "testUser";

            // First make some calculations to generate logs
            var request = new Request
            {
                Value1 = 5,
                Value2 = 3,
                User = testUser
            };

            var jsonContent = JsonSerializer.Serialize(request);
            var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Make sure the POST request succeeded
            var postResponse = await _client.PostAsync("/api/multiply", stringContent);
            Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

            // Add a small delay to ensure the log is processed
            await Task.Delay(100);

            // Act
            var response = await _client.GetAsync($"/api/multiply?user={testUser}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var logs = JsonSerializer.Deserialize<List<LogRequest>>(jsonResponse,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotEmpty(logs);
            var log = Assert.Single(logs);  // We expect exactly one log
            Assert.Equal(testUser, log.User);
            Assert.Equal("Multiply", log.Operation);
            Assert.Equal(5m, log.InputValue1);
            Assert.Equal(3m, log.InputValue2);
            Assert.Equal(15m, log.Result);
        }

        // Test Authentication Handler
        public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            public TestAuthHandler(
                IOptionsMonitor<AuthenticationSchemeOptions> options,
                ILoggerFactory logger,
                UrlEncoder encoder,
                ISystemClock clock)
                : base(options, logger, encoder, clock)
            {
            }

            protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                var claims = new[] { new Claim(ClaimTypes.Name, "Test User") };
                var identity = new ClaimsIdentity(claims, "Test");
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, "TestScheme");

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
        }

        // In-memory test repository
        public class TestLogRepository : ILogRepository
        {
            private readonly List<LogRequest> _logs = new();
            private readonly object _lock = new();

            public async Task AddAsync(LogRequest logRequest)
            {
                lock (_lock)
                {
                    _logs.Add(logRequest);
                }
                await Task.CompletedTask;
            }

            public async Task<List<LogRequest>> GetAsync(string user, string operation)
            {
                lock (_lock)
                {
                    return _logs
                        .Where(l => l.User == user && l.Operation == operation)
                        .OrderByDescending(l => l.Timestamp)
                        .Take(20)
                        .ToList();
                }
            }
        }
    }
}