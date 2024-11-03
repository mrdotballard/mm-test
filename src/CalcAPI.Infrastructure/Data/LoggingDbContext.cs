using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalcAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CalcAPI.Infrastructure.Data;
public class LoggingDbContext(DbContextOptions<LoggingDbContext> options) : DbContext(options)
{
    public DbSet<LogRequest> LogRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LogRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.User).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Operation).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Timestamp).HasDefaultValueSql("datetime('now')");
            entity.Property(e => e.InputValue1).HasPrecision(10, 2);
            entity.Property(e => e.InputValue2).HasPrecision(10, 2);
            entity.Property(e => e.Result).HasPrecision(10, 2);
        });
    }
}
