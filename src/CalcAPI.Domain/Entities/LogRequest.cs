using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalcAPI.Domain.Entities;

/// <summary>
/// EF Entity for the LogRequest DB table .
/// </summary>
public class LogRequest
{
    public int Id { get; set; }
    public string User { get; set; }
    public string Operation { get; set; }
    public DateTime Timestamp { get; set; }
    public decimal InputValue1 { get; set; }
    public decimal InputValue2 { get; set; }
    public decimal Result { get; set; }
}
