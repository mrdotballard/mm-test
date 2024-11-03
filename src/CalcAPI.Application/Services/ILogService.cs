using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalcAPI.Domain.Entities;

namespace CalcAPI.Application.Services;

public interface ILogService
{
    Task LogRequestAsync(string user, string operation, decimal inputValue1, decimal inputValue2, decimal result);
    Task<List<LogRequest>> GetLogsAsync(string user);
}