using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewFinal.Models;
using NewFinal.Models.Abilities;
using NewFinal.Models.Atributes;
using NewFinal.Models.Characters;
using NewFinal.Models.Characters.Enemy;
using NewFinal.Models.Equipments;

using System.Collections.Generic;
using System.Linq;


namespace NewFinal.Data;

public class GameContext : DbContext
{
    //public static ILoggerFactory MyLoggerFactory 
    //    = LoggerFactory.Create(b => b.AddConsole());

    public static ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder =>
    {
        builder.ClearProviders();
        builder.AddFile("efcore-log.txt");  // Using Serilog.Extensions.Logging.File
    });

    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<MagicAbility> MagicAbilities { get; set; }
    public DbSet<PhysicalAbility> PhysicalAbilities { get; set; }
    public DbSet<Monster> Monsters { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options 
            .UseSqlServer("Server=bitsql.wctc.edu;Database=FinalDBData_20023_NJK;User Id=nkelly3;Password=000458363;")
            .UseLazyLoadingProxies()
            .UseLoggerFactory(MyLoggerFactory)
            .EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure self-referencing relationships for Room cardinal directions
        modelBuilder.Entity<Room>()
            .HasOne(r => r.NorthRoom)
            .WithMany()
            .HasForeignKey(r => r.NorthRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Room>()
            .HasOne(r => r.SouthRoom)
            .WithMany()
            .HasForeignKey(r => r.SouthRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Room>()
            .HasOne(r => r.EastRoom)
            .WithMany()
            .HasForeignKey(r => r.EastRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Room>()
            .HasOne(r => r.WestRoom)
           .WithMany()
           .HasForeignKey(r => r.WestRoomId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure one-to-many relationship between Player and Room   
        modelBuilder.Entity<Player>()
            .HasOne(p => p.Room)
            .WithMany(r => r.Players)
            .HasForeignKey(p => p.RoomId)
            .OnDelete(DeleteBehavior.SetNull);
        // Configure one-to-many relationship between Player and Item
        modelBuilder.Entity<Player>()
            .HasMany(p => p.Inventory)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
        // Configure one-to-many relationship between Player and Equipment
        modelBuilder.Entity<Player>()
            .HasOne(p => p.Equipment)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
        // Configure one-to-many relationship between Player and Ability
        modelBuilder.Entity<Player>()
            .HasMany(p => p.Abilities)
            .WithOne()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ability>()
            .HasMany(a => a.Players)
            .WithOne()
            .OnDelete(DeleteBehavior.Restrict);
        // Configure one-to-many relationship between Room and Monster
        modelBuilder.Entity<Room>()
            .HasMany(r => r.Monsters)
            .WithOne(m => m.Room)
            .HasForeignKey(m => m.RoomId)
            .OnDelete(DeleteBehavior.Cascade);


    }
}
