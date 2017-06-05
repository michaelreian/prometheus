using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Prometheus.Core
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
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
                context.Database.EnsureCreated();
            }
        }
    }
}