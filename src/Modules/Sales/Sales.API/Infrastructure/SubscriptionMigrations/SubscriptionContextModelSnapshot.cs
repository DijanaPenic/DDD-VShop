﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Infrastructure;

#nullable disable

namespace VShop.Modules.Sales.API.Infrastructure.SubscriptionMigrations
{
    [DbContext(typeof(SubscriptionContext))]
    partial class SubscriptionContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("VShop.SharedKernel.EventStoreDb.Subscriptions.Infrastructure.Entities.Checkpoint", b =>
                {
                    b.Property<string>("SubscriptionId")
                        .HasColumnType("text")
                        .HasColumnName("subscription_id");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_created_utc");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_updated_utc");

                    b.Property<decimal?>("Position")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("position");

                    b.HasKey("SubscriptionId")
                        .HasName("pk_checkpoint");

                    b.ToTable("checkpoint", "subscription");
                });
#pragma warning restore 612, 618
        }
    }
}
