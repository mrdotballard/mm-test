using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalcAPI.Domain.Entities;

namespace CalcAPI.Domain.Interfaces;

public interface ILogRepository
{
    Task AddAsync(LogRequest logRequest);
    Task<List<LogRequest>> GetAsync(string user);
}
