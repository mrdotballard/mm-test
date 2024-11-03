using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalcAPI.Domain.Entities;

/// <summary>
/// Incomming request DTO
/// </summary>
public class AddRequest
{
    public decimal Value1 { get; set; }
    public decimal Value2 { get; set; }
    public string Operation { get; set; }
    public string User { get; set; }
}

/// <summary>
/// Outgoing response DTO
/// </summary>
public class AddResponse
{
    public decimal Result { get; set; }
}