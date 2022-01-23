﻿// <auto-generated />

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using VShop.SharedKernel.EventStoreDb.Subscriptions.DAL;

#nullable disable

namespace VShop.Modules.Sales.Infrastructure.Migrations.Subscription
{
    [DbContext(typeof(SubscriptionDbContext))]
    [Migration("20211209214933_ChangedDatetimeColumnsTypeAndName")]
    partial class ChangedDatetimeColumnsTypeAndName
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.HasKey("SubscriptionId")
                        .HasName("pk_checkpoint");

                    b.ToTable("checkpoint", "subscription");
                });
#pragma warning restore 612, 618
        }
    }
}
