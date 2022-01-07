﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Infrastructure;

#nullable disable

namespace VShop.Modules.Billing.API.Infrastructure.SubscriptionMigrations
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

                    b.Property<Instant>("DateCreated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_created");

                    b.Property<Instant>("DateUpdated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_updated");

                    b.Property<decimal?>("Position")
                        .HasColumnType("numeric(20,0)")
                        .HasColumnName("position");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("SubscriptionId")
                        .HasName("pk_checkpoint");

                    b.ToTable("checkpoint", "subscription");
                });

            modelBuilder.Entity("VShop.SharedKernel.EventStoreDb.Subscriptions.Infrastructure.Entities.MessageDeadLetterLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Instant>("DateCreated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_created");

                    b.Property<Instant>("DateUpdated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_updated");

                    b.Property<string>("Error")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("error");

                    b.Property<string>("MessageData")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("message_data");

                    b.Property<Guid>("MessageId")
                        .HasColumnType("uuid")
                        .HasColumnName("message_id");

                    b.Property<string>("MessageType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("message_type");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<string>("StreamId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("stream_id");

                    b.Property<uint>("xmin")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id")
                        .HasName("pk_message_dead_letter_queue");

                    b.ToTable("message_dead_letter_queue", "subscription");
                });
#pragma warning restore 612, 618
        }
    }
}
