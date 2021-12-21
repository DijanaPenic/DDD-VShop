﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using VShop.Modules.Billing.Infrastructure;

#nullable disable

namespace VShop.Modules.Billing.API.Infrastructure.Migrations
{
    [DbContext(typeof(BillingContext))]
    [Migration("20211221190336_EnabledXminConcurrenyToken")]
    partial class EnabledXminConcurrenyToken
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("VShop.Modules.Billing.Infrastructure.Entities.PaymentTransfer", b =>
                {
                    b.Property<Guid>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("order_id");

                    b.Property<Instant>("DateCreated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_created");

                    b.Property<Instant>("DateUpdated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_updated");

                    b.Property<string>("Error")
                        .HasColumnType("text")
                        .HasColumnName("error");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("OrderId")
                        .HasName("pk_payment_transfer");

                    b.ToTable("payment_transfer", "payment");
                });
#pragma warning restore 612, 618
        }
    }
}
