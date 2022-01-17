﻿// <auto-generated />

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using VShop.SharedKernel.Integration.Infrastructure;

#nullable disable

namespace VShop.Modules.Billing.Infrastructure.Migrations.Integration
{
    [DbContext(typeof(IntegrationDbContext))]
    [Migration("20211209215129_ChangedDatetimeColumnsTypeAndName")]
    partial class ChangedDatetimeColumnsTypeAndName
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("VShop.SharedKernel.Integration.Infrastructure.Entities.IntegrationEventLog", b =>
                {
                    b.Property<Guid>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("event_id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("content");

                    b.Property<Instant>("DateCreated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_created");

                    b.Property<Instant>("DateUpdated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_updated");

                    b.Property<string>("EventTypeName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("event_type_name");

                    b.Property<int>("State")
                        .HasColumnType("integer")
                        .HasColumnName("state");

                    b.Property<int>("TimesSent")
                        .HasColumnType("integer")
                        .HasColumnName("times_sent");

                    b.Property<Guid>("TransactionId")
                        .HasColumnType("uuid")
                        .HasColumnName("transaction_id");

                    b.HasKey("EventId")
                        .HasName("pk_integration_event_log");

                    b.ToTable("integration_event_log", "integration");
                });
#pragma warning restore 612, 618
        }
    }
}
