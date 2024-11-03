using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CalcAPI.Domain.Entities;

/// <summary>
/// Incomming request DTO
/// </summary>
public class Request
{
    [Required(ErrorMessage = "Value1 is required")]
    public decimal? Value1 { get; set; }
    [Required(ErrorMessage = "Value2 is required")]
    public decimal? Value2 { get; set; }
    [Required]
    [StringLength(30)]
    public string User { get; set; }
}

/// <summary>
/// Outgoing response DTO
/// </summary>
public class Response
{
    public decimal Result { get; set; }
}