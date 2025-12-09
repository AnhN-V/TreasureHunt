using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using TreasureHunt.Domain.Entities;

namespace TreasureHunt.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<TreasureHuntResult> TreasureHuntResult { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Add your model configurations here

    }
}
