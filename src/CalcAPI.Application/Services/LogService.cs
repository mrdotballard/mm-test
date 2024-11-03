using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalcAPI.Domain.Entities;
using CalcAPI.Domain.Interfaces;

namespace CalcAPI.Application.Services;

/// <summary>
/// A service for logging to the database a caclulation request.
/// </summary>
/// <remarks>
/// This service receives all pepared data from the controller and passes it to the repository (other than)
/// </remarks>
public class LogService(ILogRepository logRepository) : ILogService
{
    public async Task LogRequestAsync(string user, string operation, decimal inputValue1, decimal inputValue2, decimal result)
    {
        var logRequest = new LogRequest
        {
            User = user,
            Operation = operation,
            Timestamp = DateTime.UtcNow,
            InputValue1 = inputValue1,
            InputValue2 = inputValue2,
            Result = result
        };

        // Save the log request to the database
        await logRepository.AddAsync(logRequest);
    }

    public async Task<List<LogRequest>> GetLogsAsync(string user, string operation)
    {
        return await logRepository.GetAsync(user, operation);
    }
}