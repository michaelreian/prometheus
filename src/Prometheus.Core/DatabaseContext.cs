using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Prometheus.Core.Entities;
using Serilog;

namespace Prometheus.Core
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Setting> Setting { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Setting>().HasKey(t => t.Name);
        }
    }

    public class DatabaseConfigurationSource : IConfigurationSource
    {
        private readonly Action<DbContextOptionsBuilder> action;

        public DatabaseConfigurationSource(Action<DbContextOptionsBuilder> action)
        {
            this.action = action;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DatabaseConfigurationProvider(action);
        }
    }

    public class DatabaseConfigurationProvider : ConfigurationProvider
    {
        private readonly Action<DbContextOptionsBuilder> action;

        public DatabaseConfigurationProvider(Action<DbContextOptionsBuilder> action)
        {
            this.action = action;
        }

        public override void Load()
        {
            var builder = new DbContextOptionsBuilder<DatabaseContext>();

            this.action(builder);

            using (var context = new DatabaseContext(builder.Options))
            {
                try
                {
                    this.Data = context.Setting.ToDictionary(x => x.Name, x => x.Value);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Warning! Unable to load database settings; { e.Message }");
                }
            }
        }
    }
}