using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Prometheus.Core;

namespace Prometheus.Core.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20170605114656_DatabaseMigrations1496663205")]
    partial class DatabaseMigrations1496663205
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("Prometheus.Core.Entities.Setting", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(256);

                    b.Property<string>("Value")
                        .HasMaxLength(2048);

                    b.HasKey("Name");

                    b.ToTable("setting","public");
                });
        }
    }
}
