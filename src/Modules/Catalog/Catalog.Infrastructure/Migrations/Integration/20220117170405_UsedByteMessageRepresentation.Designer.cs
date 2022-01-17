﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using VShop.SharedKernel.Integration.Infrastructure;

#nullable disable

namespace VShop.Modules.Catalog.Infrastructure.Migrations.Integration
{
    [DbContext(typeof(IntegrationContext))]
    [Migration("20220117170405_UsedByteMessageRepresentation")]
    partial class UsedByteMessageRepresentation
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("VShop.SharedKernel.Integration.Infrastructure.Entities.IntegrationEventLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<byte[]>("Body")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("body");

                    b.Property<Instant>("DateCreated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_created");

                    b.Property<Instant>("DateUpdated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_updated");

                    b.Property<byte[]>("Metadata")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("metadata");

                    b.Property<int>("State")
                        .HasColumnType("integer")
                        .HasColumnName("state");

                    b.Property<int>("TimesSent")
                        .HasColumnType("integer")
                        .HasColumnName("times_sent");

                    b.Property<Guid>("TransactionId")
                        .HasColumnType("uuid")
                        .HasColumnName("transaction_id");

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("type_name");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id")
                        .HasName("pk_integration_event_queue");

                    b.ToTable("integration_event_queue", "integration");
                });
#pragma warning restore 612, 618
        }
    }
}
