using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalcAPI.Domain.Entities;
using CalcAPI.Domain.Interfaces;
using CalcAPI.Infrastructure.Data;

namespace CalcAPI.Infrastructure.Repositories;

public class LogRepository(LoggingDbContext context) : ILogRepository
{
    public async Task AddAsync(LogRequest logRequest)
    {
        await context.LogRequests.AddAsync(logRequest);
        await context.SaveChangesAsync();
    }
}
