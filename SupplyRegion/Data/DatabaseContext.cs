using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using SupplyRegion.Model;

namespace SupplyRegion.Data;

public class DatabaseContext : DbContext
{
    public DbSet<PurchaseRequest> PurchaseRequests { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
        string databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "regionsupply.db");
        optionsBuilder.UseSqlite($"Data Source={databasePath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<PurchaseRequest>()
            .Property(p => p.EstimatedPrice)
            .HasPrecision(18, 2);

        /*
        modelBuilder.Entity<PurchaseRequest>().HasData(
            new PurchaseRequest
            {
                Id = 1,
                CreatedDate = DateTime.Now.AddDays(-5),
                Initiator = "Иванов И.И.",
                Department = "Отдел IT",
                ProductName = "Монитор 24\"",
                Quantity = 3,
                EstimatedPrice = 15000.00m,
                Status = PurchaseStatus.Ordered
            },
            new PurchaseRequest
            {
                Id = 2,
                CreatedDate = DateTime.Now.AddDays(-3),
                Initiator = "Петрова А.С.",
                Department = "Бухгалтерия",
                ProductName = "Принтер лазерный",
                Quantity = 1,
                EstimatedPrice = 22000.00m,
                Status = PurchaseStatus.Approved
            },
            new PurchaseRequest
            {
                Id = 3,
                CreatedDate = DateTime.Now.AddDays(-1),
                Initiator = "Сидоров В.П.",
                Department = "Производство",
                ProductName = "Стол офисный",
                Quantity = 5,
                EstimatedPrice = 7500.00m,
                Status = PurchaseStatus.New
            }
        );
        */
    }
}
