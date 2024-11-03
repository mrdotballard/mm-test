using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalcAPI.Domain.Entities;
using CalcAPI.Domain.Interfaces;
using CalcAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CalcAPI.Infrastructure.Repositories;

public class LogRepository(LoggingDbContext context) : ILogRepository
{
    public async Task AddAsync(LogRequest logRequest)
    {
        await context.LogRequests.AddAsync(logRequest);
        await context.SaveChangesAsync();
    }

    public async Task<List<LogRequest>> GetAsync(string user)
    {
        var logRequests = await context.LogRequests
            .Where(lr => lr.User == user)
            .OrderByDescending(lr => lr.Timestamp)
            .Take(20)
            .ToListAsync();

        return logRequests;
    }
}
