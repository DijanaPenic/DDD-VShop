// <auto-generated />

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using VShop.Modules.Sales.Infrastructure.DAL;

namespace VShop.Modules.Sales.Infrastructure.DAL.Migrations.Sales
{
    [DbContext(typeof(SalesDbContext))]
    [Migration("20211101172935_AddedAdditionalColumnsToOrderFulfillmentProcessTable")]
    partial class AddedAdditionalColumnsToOrderFulfillmentProcessTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("VShop.Modules.Sales.Infrastructure.Entities.OrderFulfillmentProcess", b =>
                {
                    b.Property<Guid>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("order_id");

                    b.Property<DateTime>("DateCreatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_created_utc");

                    b.Property<DateTime>("DateUpdatedUtc")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_updated_utc");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<Guid>("ShoppingCartId")
                        .HasColumnType("uuid")
                        .HasColumnName("shopping_cart_id");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.HasKey("OrderId")
                        .HasName("pk_order_fulfillment_process");

                    b.ToTable("order_fulfillment_process", "order");
                });

            modelBuilder.Entity("VShop.Modules.Sales.Infrastructure.Entities.ShoppingCartInfo", b =>
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
                        .HasName("pk_shopping_cart_info");

                    b.ToTable("shopping_cart_info", "shopping_cart");
                });

            modelBuilder.Entity("VShop.Modules.Sales.Infrastructure.Entities.ShoppingCartInfoItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

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

                    b.Property<Guid>("ShoppingCartInfoId")
                        .HasColumnType("uuid")
                        .HasColumnName("shopping_cart_info_id");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("numeric")
                        .HasColumnName("unit_price");

                    b.HasKey("Id")
                        .HasName("pk_shopping_cart_info_product_item");

                    b.HasIndex("ShoppingCartInfoId")
                        .HasDatabaseName("ix_shopping_cart_info_product_item_shopping_cart_info_id");

                    b.ToTable("shopping_cart_info_product_item", "shopping_cart");
                });

            modelBuilder.Entity("VShop.Modules.Sales.Infrastructure.Entities.ShoppingCartInfoItem", b =>
                {
                    b.HasOne("VShop.Modules.Sales.Infrastructure.Entities.ShoppingCartInfo", "ShoppingCartInfo")
                        .WithMany("Items")
                        .HasForeignKey("ShoppingCartInfoId")
                        .HasConstraintName("fk_shopping_cart_info_product_item_shopping_cart_info_shopping")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ShoppingCartInfo");
                });

            modelBuilder.Entity("VShop.Modules.Sales.Infrastructure.Entities.ShoppingCartInfo", b =>
                {
                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}
