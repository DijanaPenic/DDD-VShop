﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using VShop.Modules.Sales.Infrastructure;

namespace VShop.Modules.Sales.API.Infrastructure.Migrations
{
    [DbContext(typeof(SalesContext))]
    [Migration("20211021131549_AddedTimestampFields")]
    partial class AddedTimestampFields
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("VShop.Modules.Sales.Infrastructure.Entities.BasketDetails", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uuid")
                        .HasColumnName("customer_id");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_created_utc");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_updated_utc");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.HasKey("Id")
                        .HasName("pk_basket_details");

                    b.ToTable("basket_details", "basket");
                });

            modelBuilder.Entity("VShop.Modules.Sales.Infrastructure.Entities.BasketDetailsProductItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("BasketDetailsId")
                        .HasColumnType("uuid")
                        .HasColumnName("basket_details_id");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_created_utc");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_updated_utc");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid")
                        .HasColumnName("product_id");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer")
                        .HasColumnName("quantity");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("numeric")
                        .HasColumnName("unit_price");

                    b.HasKey("Id")
                        .HasName("pk_basket_details_product_item");

                    b.HasIndex("BasketDetailsId")
                        .HasDatabaseName("ix_basket_details_product_item_basket_details_id");

                    b.ToTable("basket_details_product_item", "basket");
                });

            modelBuilder.Entity("VShop.Modules.Sales.Infrastructure.Entities.BasketDetailsProductItem", b =>
                {
                    b.HasOne("VShop.Modules.Sales.Infrastructure.Entities.BasketDetails", "BasketDetails")
                        .WithMany("ProductItems")
                        .HasForeignKey("BasketDetailsId")
                        .HasConstraintName("fk_basket_details_product_item_basket_details_basket_details_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BasketDetails");
                });

            modelBuilder.Entity("VShop.Modules.Sales.Infrastructure.Entities.BasketDetails", b =>
                {
                    b.Navigation("ProductItems");
                });
#pragma warning restore 612, 618
        }
    }
}
