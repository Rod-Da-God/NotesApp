using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal;


namespace DataAccess
{
    public static class NpgsqlModelBuilderExtensions
    {
        public static PropertyBuilder<T> HasPostgresEnumConversion<T>(this PropertyBuilder<T> builder)
            where T : struct, Enum
        {
            return builder.HasConversion(
                new ValueConverter<T, string>(
                    v => v.ToString(),
                    v => (T)Enum.Parse(typeof(T), v)
                ),
                new ValueComparer<T>(
                    (l, r) => l.Equals(r),
                    v => v.GetHashCode(),
                    v => v
                )
            ).HasColumnType(typeof(T).Name.ToLower());
        }
    }
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Tasks> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            options.UseNpgsql(config.GetConnectionString("PostgreSQL"));
        }
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Tasks>().ToTable("tasks");
            

            modelBuilder.Entity<User>()
                .Property(e => e.Role)
                .HasConversion<string>();
            modelBuilder.HasPostgresEnum<Models.TaskStatus>();

            modelBuilder.Entity<Tasks>()
                .Property(e => e.Status)
                .HasPostgresEnumConversion<Models.TaskStatus>();


            base.OnModelCreating(modelBuilder);
        }
    }
}
