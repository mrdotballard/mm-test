using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalcAPI.Domain.Entities;

public class AddRequest
{
    public decimal Value1 { get; set; }
    public decimal Value2 { get; set; }
    public string Operation { get; set; }
}

public class AddResponse
{
    public decimal Result { get; set; }
}